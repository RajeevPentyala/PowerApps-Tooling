// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using CAT.Formulas.Tools.IR;
using System.Text;

namespace CAT.Formulas.Tools;

/// <summary>
/// An Error or warning encountered while doing a source operation. 
/// </summary>
public class Error
{
    internal ErrorCode Code;
    internal SourceLocation Span;

    public string Message;

    internal Error(ErrorCode code, SourceLocation span, string message)
    {
        Span = span;
        Message = message;
        Code = code;
    }

    public bool IsError => Code.IsError();
    public bool IsWarning => !IsError;

    public override string ToString()
    {
        var sb = new StringBuilder();
        if (IsError)
        {
            sb.Append("Error   ");
        }
        else
        {
            sb.Append("Warning ");
        }
        sb.Append($"PA{(int)Code}: ");
        sb.Append(Message);

        return sb.ToString();
    }
}
