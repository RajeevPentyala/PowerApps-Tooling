// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace CAT.PowerApps.Persistence.Yaml;

public record YamlSerializerOptions
{
    public bool IsTextFirst { get; init; } = true;

    public static readonly YamlSerializerOptions Default = new()
    {
        IsTextFirst = true
    };
}
