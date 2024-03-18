// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using CAT.Formulas.Tools.Yaml;

namespace PAModelTests.YamlSerializerTests;

[YamlObject(Name = "Invalid Object")]
internal class InvalidObjectWithDuplicateNames
{
    [YamlProperty(Name = "Duplicate Name")]
    public string FirstName { get; set; }

    [YamlProperty(Name = "Duplicate Name")]
    public string LastName { get; set; }
}
