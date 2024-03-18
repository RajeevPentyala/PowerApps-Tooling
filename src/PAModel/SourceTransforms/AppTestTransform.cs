// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using CAT.Formulas.Tools.EditorState;
using CAT.Formulas.Tools.Extensions;
using CAT.Formulas.Tools.IR;
using Newtonsoft.Json;
using System.Linq;

namespace CAT.Formulas.Tools.SourceTransforms;

internal class AppTestTransform : IControlTemplateTransform
{
    private class TestStepsMetadataJson
    {
        public string Description { get; set; }
        public string Rule { get; set; }
        public string ScreenId { get; set; }
    }

    private static readonly IEnumerable<string> _targets = new List<string>() { "TestCase" };
    public IEnumerable<string> TargetTemplates => _targets;

    private const string _metadataPropName = "TestStepsMetadata";
    private readonly string _testStepTemplateName;

    // Key is UniqueId, Value is ScreenName
    private readonly IList<KeyValuePair<string, string>> _screenIdToScreenName;
    private readonly ErrorContainer _errors;

    // To hold entropy passed in by constructor
    private readonly Entropy _entropy;

    public static bool IsTestSuite(string templateName)
    {
        return templateName == "TestSuite";
    }

    public AppTestTransform(CanvasDocument app, ErrorContainer errors, TemplateStore templateStore, EditorStateStore stateStore, Entropy entropy)
    {
        _testStepTemplateName = "TestStep";

        var i = 1;
        while (templateStore.TryGetTemplate(_testStepTemplateName, out _))
            _testStepTemplateName = "TestStep" + i;

        _screenIdToScreenName = app._screens
            .Select(screen => new KeyValuePair<string, string>(app._idRestorer.GetControlId(screen.Key).ToString(), screen.Key)).ToList();

        _entropy = entropy;
        _errors = errors;
    }

    public void AfterRead(BlockNode control)
    {
        var properties = control.Properties.ToDictionary(prop => prop.Identifier);
        if (!properties.TryGetValue(_metadataPropName, out var metadataProperty))
        {
            // If no metadata props, TestStepsMetadata nonexistent
            _entropy.DoesTestStepsMetadataExist = false;

            // If the test studio is opened, but no tests are created, it's possible for a test case to exist without any
            // steps or teststepmetadata. In that case, write only the base properties.
            if (properties.Count == 2)
                return;

            _errors.ValidationError($"Unable to find TestStepsMetadata property for TestCase {control.Name.Identifier}");
            throw new DocumentException();
        }
        else
        {
            _entropy.DoesTestStepsMetadataExist = true;
        }
        properties.Remove(_metadataPropName);
        var metadataJsonString = metadataProperty.Expression.Expression.UnEscapePAString();
        var testStepsMetadata = JsonConvert.DeserializeObject<List<TestStepsMetadataJson>>(metadataJsonString);
        var newChildren = new List<BlockNode>();

        foreach (var testStep in testStepsMetadata)
        {
            if (!properties.TryGetValue(testStep.Rule, out var testStepProp))
            {
                _errors.ValidationError($"Unable to find corresponding property for test step {testStep.Rule} in {control.Name.Identifier}");
                throw new DocumentException();
            }

            var childProperties = new List<PropertyNode>()
                {
                    new()
                    {
                        Identifier = "Description",
                        Expression = new ExpressionNode()
                        {
                            Expression = testStep.Description.EscapePAString()
                        }
                    },
                    new()
                    {
                        Identifier = "Value",
                        Expression = testStepProp.Expression
                    }
                };

            if (testStep.ScreenId != null)
            {
                if (!_screenIdToScreenName.ToDictionary(kvp => kvp.Key, kvp => kvp.Value).TryGetValue(testStep.ScreenId, out var screenName))
                {
                    _errors.ValidationWarning($"ScreenId referenced by TestStep {testStep.Rule} in {control.Name.Identifier} could not be found");
                    var testStepRuleKey = $"{control.Name.Identifier}.{testStep.Rule}";

                    // checking if this key already exist, not an ideal situation
                    // Fallback logic just to make sure collisions are avoided.
                    if (_entropy.RuleScreenIdWithoutScreen.ContainsKey(testStepRuleKey))
                    {
                        _errors.GenericError($"RuleScreenIdWithoutScreen has a duplicate key {testStepRuleKey}");
                    }

                    _entropy.RuleScreenIdWithoutScreen.Add(testStepRuleKey, testStep.ScreenId);
                }

                childProperties.Add(new PropertyNode()
                {
                    Identifier = "Screen",
                    Expression = new ExpressionNode()
                    {
                        Expression = screenName ?? null
                    }
                });
            }

            var testStepControl = new BlockNode()
            {
                Name = new TypedNameNode()
                {
                    Identifier = testStep.Rule,
                    Kind = new TypeNode() { TypeName = _testStepTemplateName }
                },
                Properties = childProperties
            };

            properties.Remove(testStep.Rule);
            newChildren.Add(testStepControl);
        }
        control.Properties = properties.Values.ToList();
        control.Children = newChildren;
    }

    public void BeforeWrite(BlockNode control)
    {
        var testStepsMetadata = new List<TestStepsMetadataJson>();
        var doesTestStepsMetadataExist = _entropy.DoesTestStepsMetadataExist ?? true;

        foreach (var child in control.Children)
        {
            var propName = child.Name.Identifier;

            if (child.Name.Kind.TypeName != _testStepTemplateName)
            {
                _errors.ValidationError($"Only controls of type {_testStepTemplateName} are valid children of a TestCase");
                throw new DocumentException();
            }

            if (child.Properties.Count > 3)
            {
                _errors.ValidationWarning($"Test Step {propName} has unexpected properties");
            }

            var descriptionProp = child.Properties.FirstOrDefault(prop => prop.Identifier == "Description");

            if (descriptionProp == null)
            {
                _errors.ValidationError($"Test Step {propName} is missing a Description property");
                throw new DocumentException();
            }

            var valueProp = child.Properties.FirstOrDefault(prop => prop.Identifier == "Value");

            if (valueProp == null)
            {
                _errors.ValidationError($"Test Step {propName} is missing a Value property");
                throw new DocumentException();
            }

            var screenProp = child.Properties.FirstOrDefault(prop => prop.Identifier == "Screen");

            string screenId = null;

            // Lookup screenID by Name
            if (screenProp != null)
            {
                foreach (var prop in _screenIdToScreenName)
                {
                    if (prop.Value != null && prop.Value == screenProp.Expression.Expression)
                    {
                        screenId = prop.Key;
                    }
                }

                // in roundtrip scenario screenId could be null
                if (screenId == null)
                {
                    _errors.ValidationWarning($"Test Step {propName} references screen {screenProp.Expression.Expression} that is not present in the app");
                    var testStepRuleKey = $"{control.Name.Identifier}.{propName}";
                    if (_entropy.RuleScreenIdWithoutScreen.TryGetValue(testStepRuleKey, out var screenIdReference))
                    {
                        screenId = screenIdReference;
                        _entropy.RuleScreenIdWithoutScreen.Remove(testStepRuleKey);
                    }
                }
            }

            if (doesTestStepsMetadataExist)
            {
                testStepsMetadata.Add(new TestStepsMetadataJson()
                {
                    Description = descriptionProp.Expression.Expression.UnEscapePAString(),
                    Rule = propName,
                    ScreenId = screenId
                });

                control.Properties.Add(new PropertyNode()
                {
                    Expression = valueProp.Expression,
                    Identifier = propName
                });
            }
        }

        if (doesTestStepsMetadataExist)
        {
            /* When Canvas creates the TestStepsMetadata value, it does so using Newtonsoft, creating a JArray of JObjects and calling
             * the ToString method on that JArray with no special formatting. This skips escaping on a number of Unicode characters
             * (such as a no-break space). System.Text.Json allows some control of escaping, but has a global block list  which causes
             * certain Unicode characters to be escaped in all cases. As such, we use Newtonsoft for TestStepsMetadata to match the
             * behavior in Canvas and prevent roundtrip errors. The appropriate encoding will ultimately happen when the full document
             * is serialized to JSON during the creation of the msapp, and will be consistent with how Canvas serializes an msapp.
             * 
             * See: https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-character-encoding#global-block-list
             */
            var testStepMetadataStr = JsonConvert.SerializeObject(testStepsMetadata, Formatting.None);
            control.Properties.Add(new PropertyNode()
            {
                Expression = new ExpressionNode() { Expression = testStepMetadataStr.EscapePAString() },
                Identifier = _metadataPropName
            });
        }
        control.Children = new List<BlockNode>();
    }
}
