// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace CAT.PowerApps.Persistence.Yaml;

public record YamlDeserializerOptions
{
    public bool IsTextFirst { get; init; } = true;

    public static readonly YamlDeserializerOptions Default = new()
    {
        IsTextFirst = true
    };
}
