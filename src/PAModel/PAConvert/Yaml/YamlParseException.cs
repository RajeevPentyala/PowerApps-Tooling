// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace CAT.Formulas.Tools.Yaml;

public class YamlParseException : Exception
{
    public YamlParseException(string message, int line = 0, Exception innerException = null)
        : base(message, innerException)
    {
        Line = line;
    }

    public int Line { get; init; }
}
