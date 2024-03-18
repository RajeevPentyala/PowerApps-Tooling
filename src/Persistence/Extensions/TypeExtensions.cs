// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using CAT.PowerApps.Persistence.Attributes;

namespace CAT.PowerApps.Persistence.Extensions;

internal static class TypeExtensions
{
    public static bool IsFirstClass(this Type type, out FirstClassAttribute? attribute)
    {
        var attributes = type.GetCustomAttributes(true) ?? Array.Empty<Attribute>();
        attribute = attributes.FirstOrDefault(a => a is FirstClassAttribute) as FirstClassAttribute;
        return attribute is not null;
    }
}
