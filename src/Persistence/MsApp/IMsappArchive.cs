// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Compression;
using Microsoft.PowerPlatform.PowerApps.Persistence.Models;

namespace Microsoft.PowerPlatform.PowerApps.Persistence.MsApp;

/// <summary>
/// base interface for MsappArchive
/// </summary>
public interface IMsappArchive : IDisposable
{
    /// <summary>
    /// The app that is represented by the archive.
    /// </summary>
    App? App { get; set; }

    T Deserialize<T>(string entryName, bool ensureRoundTrip = true);

    /// <summary>
    /// Saves control in the archive. Control can be App, Screen, or individual control.
    /// </summary>
    void Save(Control control, string? directory = null);

    /// <summary>
    /// Saves the archive to the given stream or file.
    /// </summary>
    void Save();

    /// <summary>
    /// Total sum of decompressed sizes of all entries in the archive.
    /// </summary>
    long DecompressedSize { get; }

    /// <summary>
    /// Total sum of compressed sizes of all entries in the archive.
    /// </summary>
    long CompressedSize { get; }

    /// <summary>
    /// Creates a new entry in the archive with the given name.
    /// </summary>
    /// <param name="entryName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    ZipArchiveEntry CreateEntry(string entryName);

    /// <summary>
    /// Returns the entry in the archive with the given name.
    /// </summary>
    /// <param name="entryName"></param>
    /// <returns>the entry or null when not found.</returns>
    ZipArchiveEntry? GetEntry(string entryName);

    /// <summary>
    /// Returns the entry in the archive with the given name.
    /// </summary>
    ZipArchiveEntry GetRequiredEntry(string entryName);

    /// <summary>
    /// Returns all entries in the archive that are in the given directory.
    /// </summary>
    IEnumerable<ZipArchiveEntry> GetDirectoryEntries(string directoryName, string? extension = null, bool recursive = true);

    /// <summary>
    /// Dictionary of all entries in the archive.
    /// </summary>
    IReadOnlyDictionary<string, ZipArchiveEntry> CanonicalEntries { get; }

    /// <summary>
    /// Provides access to the underlying zip archive.
    /// Attention: This property might be removed in the future.
    /// </summary>
    ZipArchive ZipArchive { get; }
}
