using ArchiSteamFarm.Steam;
using System.Text;

namespace ASFDailyExecute.Core;

internal static class Command
{
    /// <summary>
    /// 获取脚本内容
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    public static async Task<string> ResponseGetScript()
    {
        var sb = new StringBuilder();

        var lines = await ScriptManager.LoadScriptContent().ConfigureAwait(false);

        sb.AppendLine("脚本路径:");
        sb.AppendLine(ScriptManager.GetScriptPath());
        sb.AppendLine();
        sb.AppendLine("脚本内容:");
        sb.AppendLine(string.Join("\r\n", lines));

        return FormatStaticResponse(sb.ToString());
    }

    public static async Task<string> ResponseResetScript()
    {
        await ScriptManager.ResetDefaultScript().ConfigureAwait(false);
        return FormatStaticResponse("脚本已经重置为默认值");
    }

    public static async Task<string> ResponseTest(Bot bot)
    {
        var s1 = await WebRequest.GetMarketAvailable(bot);
        var s2 = await WebRequest.GetBotGameCount(bot);

        ASFLogger.LogGenericInfo(string.Format("{0} {1}", s1, s2));

        return "ok";
    }
}