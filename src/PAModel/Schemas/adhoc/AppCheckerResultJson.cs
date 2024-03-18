// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace CAT.Formulas.Tools.Schemas;

internal class AppCheckerResultJson
{
    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtensionData { get; set; }
}
