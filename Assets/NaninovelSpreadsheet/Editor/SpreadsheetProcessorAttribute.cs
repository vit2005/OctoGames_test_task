using System;

namespace Naninovel.Spreadsheet
{
    /// <summary>
    /// When applied to a class inherited from <see cref="SpreadsheetProcessor"/> will use
    /// instance of the type instead of the base class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SpreadsheetProcessorAttribute : Attribute { }
}
