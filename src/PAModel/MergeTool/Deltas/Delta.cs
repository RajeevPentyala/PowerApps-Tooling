// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace CAT.Formulas.Tools.MergeTool.Deltas;

internal interface IDelta
{
    void Apply(CanvasDocument document);
}
