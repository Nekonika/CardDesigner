using System.Text.RegularExpressions;

namespace CardDesigner;

internal static partial class RegexHelper
{
    [GeneratedRegex("\r\n|\r|\n")]
    private static partial Regex NewLineRegex_Internal();
    
    public static Regex NewLineRegex => NewLineRegex_Internal(); 
}