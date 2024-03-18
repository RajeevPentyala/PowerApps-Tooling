// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using CAT.PowerApps.Persistence.Attributes;
using CAT.PowerApps.Persistence.Templates;
using YamlDotNet.Serialization;

namespace CAT.PowerApps.Persistence.Models;

[FirstClass(templateName: BuiltInTemplates.Screen)]
[YamlSerializable]
public record Screen : Control
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    [SetsRequiredMembers]
    public Screen(string name, IControlTemplateStore controlTemplateStore)
    {
        Name = name;
        Template = controlTemplateStore.GetByName(BuiltInTemplates.Screen);
    }
}
