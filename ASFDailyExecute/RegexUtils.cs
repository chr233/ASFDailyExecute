using System.Text.RegularExpressions;

namespace ASFDailyExecute;
internal static partial class RegexUtils
{
    [GeneratedRegex(@"\d+:\d+(?::\d+)?")]
    public static partial Regex MatchExecuteTime();

    [GeneratedRegex(@"已拥有 ([\d,]+) 款游戏")]
    public static partial Regex MatchGameCount();
}
