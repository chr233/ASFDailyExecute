using ArchiSteamFarm.Steam;
using ASFDailyExecute.Data;
using System.Collections.Concurrent;
using System.Text;

namespace ASFDailyExecute.Core;

/// <summary>
/// 卡牌套数管理类
/// </summary>
internal class AccountManager
{
    /// <summary>
    /// 卡牌套数信息缓存
    /// </summary>
    private ConcurrentDictionary<Bot, BotSummary> BotSummaryDict { get; } = [];

    public void UpdateBotSummary(Bot bot, BotSummary summary)
    {
        BotSummaryDict[bot] = summary;
    }

    private const string SaveFileName = "AccountInfo.csv";

    /// <summary>
    /// 保存缓存文件
    /// </summary>
    /// <returns></returns>
    internal async Task<bool> SaveToFile()
    {
        try
        {
            var pluginFolder = Path.GetDirectoryName(MyLocation) ?? ".";
            var filePath = Path.Combine(pluginFolder, SaveFileName);

            using var fs = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            using var sw = new StreamWriter(fs);
            var sb = new StringBuilder();

            sb.AppendLine("机器人,SteamId,市场,国家,钱包余额,游戏数量,等级");

            foreach (var (b, s) in BotSummaryDict)
            {
                sb.AppendLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",{4},{5},{6}", b.BotName, s.SteamId, s.MarketAvailable ? '√' : '×', s.Country, s.WalletBalance, s.GameCount, s.Level));
            }

            await sw.WriteAsync(sb).ConfigureAwait(false);
            await sw.FlushAsync().ConfigureAwait(false);

            ASFLogger.LogGenericError($"保存文件到 {filePath}");

            return true;
        }
        catch (Exception ex)
        {
            ASFLogger.LogGenericException(ex);
            ASFLogger.LogGenericError("保存文件失败");
            return false;
        }
    }
}