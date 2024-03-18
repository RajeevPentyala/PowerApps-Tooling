// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using CAT.PowerApps.Persistence.Attributes;
using CAT.PowerApps.Persistence.Templates;
using YamlDotNet.Serialization;

namespace CAT.PowerApps.Persistence.Models;

[FirstClass(templateName: BuiltInTemplates.Component)]
[YamlSerializable]
public record Component : Control
{
    protected Component() { }

    /// <summary>
    /// Default constructor.
    /// </summary>
    [SetsRequiredMembers]
    public Component(string name, IControlTemplateStore controlTemplateStore)
    {
        Name = name;
        Template = controlTemplateStore.GetByName(BuiltInTemplates.Component);
    }
}
