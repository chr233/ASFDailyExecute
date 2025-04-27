namespace ASFDailyExecute.Data;
internal sealed record BotSummary
{
    public BotSummary(bool marketAvailable, ulong steamId, string country, decimal walletBalance, int gameCount, int level)
    {
        MarketAvailable = marketAvailable;
        SteamId = steamId;
        Country = country;
        WalletBalance = walletBalance;
        GameCount = gameCount;
        Level = level;
        CreateAt = DateOnly.FromDateTime(DateTime.Now);
    }

    /// <summary>
    /// 市场是否可用
    /// </summary>
    public bool MarketAvailable { get; set; }

    public ulong SteamId { get; set; }

    public string Country { get; set; } = "";
    public decimal WalletBalance { get; set; }
    public int GameCount { get; set; }
    public int Level { get; set; }
    public DateOnly CreateAt { get; set; }
}
