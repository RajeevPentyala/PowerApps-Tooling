// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace CAT.PowerApps.Persistence;

public class PersistenceException : Exception
{
    public required string FileName { get; init; }

    public int Line { get; init; }

    public int Column { get; init; }

    public PersistenceException(string message)
        : base(message)
    {
    }

    public PersistenceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
