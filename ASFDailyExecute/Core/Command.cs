using System.Text;

namespace ASFDailyExecute.Core;

internal static class Command
{
    /// <summary>
    /// 获取脚本内容
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    public static async Task<string?> ResponseGetScript()
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

    public static async Task<string?> ResponseResetScript()
    {
        await ScriptManager.ResetDefaultScript().ConfigureAwait(false);
        return FormatStaticResponse("脚本已经重置为默认值");
    }
}