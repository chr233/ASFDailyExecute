namespace ASFDailyExecute.Core;
internal sealed class ScriptManager
{
    private Timer? timer;

    private TimeOnly ExecuteTime = new TimeOnly(0, 0, 0);
    private DateOnly LastRun = DateOnly.MinValue;

    private SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1);

    public void Init()
    {
        var time = Config.ExecuteTime;

        if (RegexUtils.MatchExecuteTime().IsMatch(time) && TimeOnly.TryParse(time, out var actTime))
        {
            ExecuteTime = actTime;
#if DEBUG
            timer = new Timer(TimerCallback, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(5));
#else
            timer = new Timer(TimerCallback, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
#endif
        }
        else
        {
            ASFLogger.LogGenericWarning("ASFEnhance.ExecuteTime 配置无效, 示例配置 00:00");
        }
    }

    private const string ScriptName = "ASFDailyExecute.txt";

    /// <summary>
    /// 加载脚本内容
    /// </summary>
    /// <returns></returns>
    public static async Task<string[]> LoadScript()
    {
        var filePath = Path.Combine(MyDirectory, ScriptName);
        await CreateDefaultScript(filePath).ConfigureAwait(false);

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

    private static async Task CreateDefaultScript(string filePath)
    {
        if (File.Exists(filePath))
        {
            return;
        }

        ASFLogger.LogGenericWarning("脚本不存在, 创建默认脚本模板");

        using var writer = new StreamWriter(filePath, false, encoding: System.Text.Encoding.UTF8);
        writer.WriteLine("#");
        writer.WriteLine("# 脚本说明 by chr_");
        writer.WriteLine("#");
        writer.WriteLine("# 1. # 后面的内容会被忽略");
        writer.WriteLine("# 2. $ 会被替换为机器人名");
        writer.WriteLine("# 3. 脚本一行一句");
        writer.WriteLine();
        writer.WriteLine("STATUS $");

        await writer.FlushAsync().ConfigureAwait(false);
        writer.Close();
    }

    public async void TimerCallback(object? _)
    {
        ASFLogger.LogGenericWarning(DateTime.Now.ToString());

        if (DateOnly.FromDateTime(DateTime.Now) == LastRun)
        {
            return;
        }

        await BotTask().ConfigureAwait(false);
    }

    private async Task BotTask()
    {
        await SemaphoreSlim.WaitAsync().ConfigureAwait(false);

        try
        {




        }
        finally
        {
            SemaphoreSlim.Release();
        }
    }
}
