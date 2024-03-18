// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using CAT.PowerApps.Persistence.Attributes;
using CAT.PowerApps.Persistence.Templates;
using YamlDotNet.Serialization;

namespace CAT.PowerApps.Persistence.Models;

/// <summary>
/// Represents an Canvas App.
/// </summary>
[FirstClass(templateName: BuiltInTemplates.App)]
[YamlSerializable]
public record App : Control
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    [SetsRequiredMembers]
    public App(string name, IControlTemplateStore controlTemplateStore)
    {
        Name = name;
        Template = controlTemplateStore.GetByName(BuiltInTemplates.App);
    }

    [YamlIgnore]
    public IList<Screen> Screens { get; set; } = new List<Screen>();
}
