using ArchiSteamFarm.Core;
using ArchiSteamFarm.Localization;
using ArchiSteamFarm.Steam;

namespace ASFDailyExecute.Core;

internal static class Command
{
    /// <summary>
    /// 停止游玩随机游戏
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    internal static async Task<string?> ResponseGetScript(Bot bot)
    {
        var lines = await ScriptMgr.LoadScript().ConfigureAwait(false);

        var text = string.Join("\r\n", lines);

        return text;
    }

    /// <summary>
    /// 停止游玩随机游戏 (多个Bot)
    /// </summary>
    /// <param name="botNames"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    internal static async Task<string?> ResponseGetScript(string botNames)
    {
        if (string.IsNullOrEmpty(botNames))
        {
            throw new ArgumentNullException(nameof(botNames));
        }

        var bots = Bot.GetBots(botNames);

        if (bots == null || bots.Count == 0)
        {
            return FormatStaticResponse(Strings.BotNotFound, botNames);
        }

        var results = await Utilities.InParallel(bots.Select(ResponseGetScript))
            .ConfigureAwait(false);
        var responses = new List<string>(results.Where(static result => !string.IsNullOrEmpty(result))!);

        return responses.Count > 0 ? string.Join(Environment.NewLine, responses) : null;
    }
}