// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;

namespace CAT.PowerApps.Persistence.Models;

internal record AppThemes
{
    [SetsRequiredMembers]
    public AppThemes()
    {
    }

    public required string CurrentTheme { get; init; } = "defaultTheme";
    public IList<object> CustomThemes { get; init; } = new List<object>();
}
