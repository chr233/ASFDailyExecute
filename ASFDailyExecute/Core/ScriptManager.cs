namespace ASFDailyExecute.Core;
internal sealed class ScriptManager
{
    private Timer? timer;

    private DateOnly LastRun = DateOnly.MinValue;
    private SemaphoreSlim semaphoreSlim;

    public void Init()
    {
        var time = Config.ExecuteTime;

        if (RegexUtils.MatchExecuteTime().IsMatch(time) && TimeOnly.TryParse(time, out var actTime))
        {
            timer = new Timer(TimerCallback, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(10));
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
    public async Task<string[]> LoadScript()
    {
        var filePath = Path.Combine(MyDirectory, ScriptName);
        await CreateDefaultScript(filePath).ConfigureAwait(false);

        using var reader = new StreamReader(filePath, encoding: System.Text.Encoding.UTF8);
        var content = await reader.ReadToEndAsync().ConfigureAwait(false);

        reader.Close();

        var lines = content.Split(Environment.NewLine);

        return lines;
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
    }

    private async Task BotTask(SemaphoreSlim semaphore)
    {
    }
}
