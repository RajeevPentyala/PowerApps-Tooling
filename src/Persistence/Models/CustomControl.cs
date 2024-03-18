// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using CAT.PowerApps.Persistence.Templates;

namespace CAT.PowerApps.Persistence.Models;

public record CustomControl : Control
{
    public CustomControl()
    {
    }

    [SetsRequiredMembers]
    public CustomControl(string name, ControlTemplate controlTemplate) : base(name, controlTemplate)
    {
    }
}
