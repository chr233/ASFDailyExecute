using System.Text.RegularExpressions;

namespace ASFDailyExecute;
internal static partial class RegexUtils
{
    [GeneratedRegex(@"\d+:\d+(?::\d+)?")]
    public static partial Regex MatchExecuteTime();
}
