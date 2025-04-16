namespace ASFDailyExecute.Data;

internal sealed record PluginConfig(
    bool EULA,
    bool Statistic = true,
    string ExecuteTime = "00:00",
    bool OfflineAfterExecute = true);
