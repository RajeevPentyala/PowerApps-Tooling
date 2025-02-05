// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.PowerPlatform.PowerApps.Persistence.Collections;
using Microsoft.PowerPlatform.PowerApps.Persistence.Templates;
using Microsoft.PowerPlatform.PowerApps.Persistence.Yaml;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.Callbacks;

namespace Microsoft.PowerPlatform.PowerApps.Persistence.Models;

[DebuggerDisplay("{Template?.DisplayName}: {Name}")]
public abstract record Control
{
    [SuppressMessage("Style", "IDE0032:Use auto property", Justification = "We need both 'public init' and 'private set', which cannot be accomplished by auto property")]
    private IList<Control>? _children;

    public Control()
    {
    }

    [SetsRequiredMembers]
    public Control(string name, ControlTemplate template)
    {
        Name = name;
        Template = template;
    }

    /// <summary>
    /// template uri of the control.
    /// </summary>
    [YamlMember(Alias = YamlFields.Control, Order = 0)]
    public string TemplateId => Template.Id;

    private readonly string _name = string.Empty;
    /// <summary>
    /// the control's name.
    /// </summary>
    [YamlMember(Order = 1)]
    public required string Name
    {
        get => _name;
        init
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(nameof(Name));

            _name = value.Trim();
        }
    }

    /// <summary>
    /// key/value pairs of Control properties. Mapped to/from Control rules.
    /// </summary>
    [YamlMember(Order = 2)]
    public ControlPropertiesCollection Properties { get; init; } = new();

    ///// <summary>
    ///// list of child controls nested under this control.
    ///// This collection can be null in cases where the control does not support children.
    ///// </summary>
    //[YamlMember(Order = 3)]
    //public IList<Control>? Children { get => _children; set => _children = value; }

    /// <summary>
    /// List of child controls nested under this control.
    /// This collection can be null in cases where the control does not support children.
    /// </summary>
    [YamlMember(Order = 3)]
    public IList<Control>? Children { get; set; }


    [YamlIgnore]
    public ControlEditorState? EditorState { get; set; }

    [YamlIgnore]
    public required ControlTemplate Template { get; init; }

    [OnDeserialized]
    internal void AfterDeserialize()
    {
        // Apply a descending ZIndex value for each child
        if (Children == null)
            return;

        if (this is App)
            return; // Apps do not place ZIndex on their Host child

        for (var i = 0; i < Children.Count; i++)
        {
            var zIndex = Children.Count - i;
            Children[i].Properties.Set(PropertyNames.ZIndex, new(zIndex.ToString(CultureInfo.InvariantCulture)) { IsFormula = false });
        }
    }

    [OnSerializing]
    internal void BeforeSerialize()
    {
        // Children should be sorted by ZIndex (which DocServer doesn't perform), and
        // the ZIndex property should be removed as the user should only "set" this value
        // by reordering the children
        if (_children == null)
            return;

        HideNestedTemplates();

        _children = _children
            .OrderByDescending(getZIndex)
            .Select(removeZIndexProperty)
            .ToList();

        static int getZIndex(Control child) =>
            child.Properties.TryGetValue(PropertyNames.ZIndex, out var prop) && int.TryParse(prop.Value, out var zIndex)
                ? zIndex
                : int.MaxValue;

        static Control removeZIndexProperty(Control child)
        {
            child.Properties.Remove(PropertyNames.ZIndex);
            return child;
        }
    }

    /// <summary>
    /// Called before serialization to hide nested templates which add properties to parent from YAML output.
    /// </summary>
    internal void HideNestedTemplates()
    {
        if (_children == null)
            return;

        for (var i = 0; i < _children.Count; i++)
        {
            if (_children[i].Template.AddPropertiesToParent)
            {
                foreach (var childTemplateProperty in _children[i].Properties)
                {
                    Properties.Add(childTemplateProperty.Key, childTemplateProperty.Value);
                }
                _children.RemoveAt(i);
                i--;
            }
        }
    }
}
