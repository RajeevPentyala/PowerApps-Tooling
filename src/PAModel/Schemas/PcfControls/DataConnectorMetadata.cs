// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace CAT.Formulas.Tools.Schemas.PcfControl;

internal struct DataConnectorMetadata
{
    [JsonExtensionData]
    public Dictionary<string, object> ExtensionData { get; set; }
}
