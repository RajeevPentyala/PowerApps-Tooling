// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AppMagic.Authoring.Persistence;
using CAT.Formulas.Tools.Schemas;
using System.Text.Json.Serialization;
using static CAT.Formulas.Tools.ControlInfoJson;

namespace CAT.Formulas.Tools.EditorState;

// A combination of the control templates present in Templates.json and the control files
internal class CombinedTemplateState
{
    public string Id { get; set; }

    // Very important for data components.
    public string Name { get; set; }

    public string Version { get; set; }
    public string LastModifiedTimestamp { get; set; }

    // Used with templates. 
    public bool? IsComponentTemplate { get; set; }
    public bool? FirstParty { get; set; }
    public ComponentDefinitionInfoJson ComponentDefinitionInfo { get; set; }

    // Present for component templates with functions
    public CustomPropertyJson[] CustomProperties { get; set; }

    public bool? IsComponentLocked { get; set; }
    public bool? ComponentChangedSinceFileImport { get; set; }
    public bool? ComponentAllowCustomization { get; set; }
    public string TemplateOriginalName { get; set; }
    public ComponentType? ComponentType { get; set; }

    // Present on PCF
    public string TemplateDisplayName { get; set; }
    public string DynamicControlDefinitionJson { get; set; }

    public ComponentManifest ComponentManifest { get; set; }

    // Present on Legacy DataTable columns
    public string CustomControlDefinitionJson { get; set; }

    [JsonExtensionData]
    public Dictionary<string, object> ExtensionData { get; set; }

    // Maps to TemplateMetadataJson ExtensionData
    public Dictionary<string, object> ComponentExtraMetadata { get; set; }

    // Some of the xml templates have widget root node, and these are templates which are returned by the doc server in the UsedTemplates collection.
    public bool IsWidgetTemplate { get; set; }

    public bool IsPcfControl { get; set; }

    public CombinedTemplateState() { }

    public CombinedTemplateState(Template template)
    {
        Id = template.Id;
        Name = template.Name;
        Version = template.Version;
        LastModifiedTimestamp = template.LastModifiedTimestamp;
        ComponentDefinitionInfo = null;
        IsComponentTemplate = template.IsComponentDefinition;
        CustomProperties = template.CustomProperties;
        TemplateDisplayName = template.TemplateDisplayName;
        ExtensionData = template.ExtensionData;
        ComponentType = template.ComponentType;
        FirstParty = template.FirstParty;
        CustomControlDefinitionJson = template.CustomControlDefinitionJson;
        DynamicControlDefinitionJson = template.DynamicControlDefinitionJson;
        if (template.Id != null)
            IsPcfControl = template.Id.StartsWith(Template.PcfControl);
    }

    public Template ToControlInfoTemplate()
    {
        return new Template()
        {
            Id = Id,
            Name = Name,
            Version = Version,
            LastModifiedTimestamp = LastModifiedTimestamp,
            IsComponentDefinition = IsComponentTemplate,
            ComponentDefinitionInfo = ComponentDefinitionInfo,
            CustomProperties = CustomProperties,
            TemplateDisplayName = TemplateDisplayName,
            ExtensionData = ExtensionData,
            ComponentType = ComponentType,
            FirstParty = FirstParty,
            CustomControlDefinitionJson = CustomControlDefinitionJson,
            DynamicControlDefinitionJson = DynamicControlDefinitionJson
        };
    }

    public TemplateMetadataJson ToTemplateMetadata(Entropy entropy)
    {
        var version = Version;
        if (entropy.TemplateVersions.TryGetValue(Name, out var conflictVersion))
            version = conflictVersion;
        return new TemplateMetadataJson()
        {
            Name = Name,
            OriginalName = TemplateOriginalName,
            Version = version,
            CustomProperties = CustomProperties,
            IsComponentLocked = IsComponentLocked,
            ComponentType = ComponentType,
            ComponentChangedSinceFileImport = ComponentChangedSinceFileImport,
            ComponentAllowCustomization = ComponentAllowCustomization,
            ExtensionData = ComponentExtraMetadata,
        };
    }
}
