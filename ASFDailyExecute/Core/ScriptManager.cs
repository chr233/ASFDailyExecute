using ArchiSteamFarm.Steam;

namespace ASFDailyExecute.Core;
internal static class ScriptManager
{
    private static Timer? timer;

    private static TimeOnly ExecuteTime = new TimeOnly(0, 0, 0);
    private static DateOnly LastRunDate = DateOnly.MinValue;

    /// <summary>
    /// 初始化
    /// </summary>
    public static void Init()
    {
        var time = Config.ExecuteTime;

        if (RegexUtils.MatchExecuteTime().IsMatch(time) && TimeOnly.TryParse(time, out var actTime))
        {
            ExecuteTime = actTime;
#if DEBUG
            timer = new Timer(TimerCallback, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(5));
#else
            timer = new Timer(TimerCallback, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(60));
#endif
        }
        else
        {
            ASFLogger.LogGenericWarning("ASFEnhance.ExecuteTime 配置无效, 示例配置 00:00");
        }
    }

    private const string ScriptName = "ASFDailyExecute.txt";

    /// <summary>
    /// 获取脚本路径
    /// </summary>
    /// <returns></returns>
    public static string GetScriptPath()
    {
        return Path.Combine(MyDirectory, ScriptName);
    }

    /// <summary>
    /// 加载脚本内容
    /// </summary>
    /// <returns></returns>
    public static async Task<string[]> LoadScriptContent()
    {
        var filePath = GetScriptPath();
        await CreateDefaultScript().ConfigureAwait(false);

        using var reader = new StreamReader(filePath, encoding: System.Text.Encoding.UTF8);

        List<string> scripts = [];

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync().ConfigureAwait(false);

            line = line?.Trim();

            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            var index = line.IndexOf('#');

            if (index == 0)
            {
                continue;
            }

            if (index != -1)
            {
                line = line[..index].Trim();
            }

            if (!string.IsNullOrEmpty(line))
            {
                scripts.Add(line);
            }
        }
        reader.Close();

        return [.. scripts];
    }

    private const string BackupScriptName = "ASFDailyExecute.backup.txt";

    /// <summary>
    /// 获取脚本路径
    /// </summary>
    /// <returns></returns>
    private static string GetBackupScriptPath()
    {
        return Path.Combine(MyDirectory, BackupScriptName);
    }

    /// <summary>
    /// 创建默认脚本
    /// </summary>
    /// <returns></returns>
    private static Task CreateDefaultScript()
    {
        var filePath = GetScriptPath();
        if (File.Exists(filePath))
        {
            return Task.CompletedTask;
        }

        return ResetDefaultScript();
    }

    /// <summary>
    /// 重置脚本
    /// </summary>
    /// <returns></returns>
    public static async Task ResetDefaultScript()
    {
        var filePath = GetScriptPath();
        if (File.Exists(filePath))
        {
            var backupPath = GetBackupScriptPath();
            File.Move(filePath, backupPath, true);
            ASFLogger.LogGenericWarning($"备份旧脚本到 {backupPath}");
        }

        ASFLogger.LogGenericWarning("创建默认脚本模板");

        using var writer = new StreamWriter(filePath, false, encoding: System.Text.Encoding.UTF8);
        writer.WriteLine("#");
        writer.WriteLine("# 脚本说明 by chr_");
        writer.WriteLine("#");
        writer.WriteLine("# 1. # 后面的内容会被忽略");
        writer.WriteLine("# 2. $ 会被替换为机器人名");
        writer.WriteLine("# 3. 脚本一行一句");
        writer.WriteLine();
        writer.WriteLine("LEVEL $");
        writer.WriteLine("BALANCE $");
        writer.WriteLine("STATUS $");

        await writer.FlushAsync().ConfigureAwait(false);
        writer.Close();
    }

    public static async void TimerCallback(object? _)
    {
        if (DateOnly.FromDateTime(DateTime.Now) == LastRunDate)
        {
            return;
        }

        if (TimeOnly.FromDateTime(DateTime.Now) < ExecuteTime)
        {
            return;
        }

        LastRunDate = DateOnly.FromDateTime(DateTime.Now);

        await ExecuteBotScript().ConfigureAwait(false);
    }

    private static readonly SemaphoreSlim SemaphoreSlim = new(1);

    /// <summary>
    /// 执行脚本
    /// </summary>
    /// <returns></returns>
    private static async Task ExecuteBotScript()
    {
        await SemaphoreSlim.WaitAsync().ConfigureAwait(false);

        ASFLogger.LogGenericWarning(DateTime.Now.ToString());

        if (Bot.BotsReadOnly == null)
        {
            return;
        }

        try
        {
            var lines = await LoadScriptContent().ConfigureAwait(false);

            foreach (var (_, bot) in Bot.BotsReadOnly)
            {
                var botConnected = bot.IsConnectedAndLoggedOn;

                if (!botConnected)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        ASFLogger.LogGenericInfo($"{bot.BotName} 离线, 正在启动");

                        bot.Actions.Start();

                        await Task.Delay(3000).ConfigureAwait(false);

                        if (bot.IsConnectedAndLoggedOn)
                        {
                            break;
                        }
                    }
                }

                if (!bot.IsConnectedAndLoggedOn)
                {
                    ASFLogger.LogGenericInfo($"{bot.BotName} 上线失败, 跳过执行");
                    continue;
                }

                foreach (var line in lines)
                {
                    var command = line.Replace("$", bot.BotName);
                    var result = await bot.Commands.Response(EAccess.Master, command, 0).ConfigureAwait(false);

                    ASFLogger.LogGenericInfo($"{bot.BotName} 执行命令 {command}");
                    ASFLogger.LogGenericInfo($"{bot.BotName} 执行结果 {result}");
                }

                if (!botConnected && Config.OfflineAfterExecute && bot.CardsFarmer.CurrentGamesFarmingReadOnly.Count == 0)
                {
                    await bot.Actions.Stop().ConfigureAwait(false);
                }
            }
        }
        finally
        {
            SemaphoreSlim.Release();
        }
    }
}
