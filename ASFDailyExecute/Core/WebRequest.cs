using ArchiSteamFarm.Steam;
using SteamKit2;


namespace ASFDailyExecute.Core;

internal static class WebRequest
{
    /// <summary>
    /// 获取市场可用性
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    public static async Task<bool> GetMarketAvailable(Bot bot)
    {
        var request = new Uri(SteamCommunityURL, "/market/");
        var response = await bot.ArchiWebHandler.UrlGetToHtmlDocumentWithSession(request, referer: SteamCommunityURL).ConfigureAwait(false);

        if (response?.Content == null)
        {
            return false;
        }

        var eleError = response.Content.QuerySelector("div.market_headertip_container_warning");
        return eleError == null;
    }

    /// <summary>
    /// 获取机器人国家
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    public static string GetBotCountry(Bot bot)
    {
        return bot.WalletCurrency switch {
            ECurrencyCode.USD => "US",
            ECurrencyCode.GBP => "GB",
            ECurrencyCode.EUR => "EU",
            ECurrencyCode.CHF => "CH",
            ECurrencyCode.RUB => "RU",
            ECurrencyCode.PLN => "PL",
            ECurrencyCode.BRL => "BR",
            ECurrencyCode.JPY => "JP",
            ECurrencyCode.NOK => "NO",
            ECurrencyCode.IDR => "ID",
            ECurrencyCode.MYR => "MY",
            ECurrencyCode.PHP => "PH",
            ECurrencyCode.SGD => "SG",
            ECurrencyCode.THB => "TH",
            ECurrencyCode.VND => "VN",
            ECurrencyCode.KRW => "KR",
            ECurrencyCode.TRY => "TR",
            ECurrencyCode.UAH => "UA",
            ECurrencyCode.MXN => "MX",
            ECurrencyCode.CAD => "CA",
            ECurrencyCode.AUD => "CX",
            ECurrencyCode.NZD => "CK",
            ECurrencyCode.CNY => "CN",
            ECurrencyCode.INR => "IN",
            ECurrencyCode.CLP => "CL",
            ECurrencyCode.PEN => "PE",
            ECurrencyCode.COP => "CO",
            ECurrencyCode.ZAR => "ZA",
            ECurrencyCode.HKD => "HK",
            ECurrencyCode.TWD => "TW",
            ECurrencyCode.SAR => "SA",
            ECurrencyCode.AED => "AE",
            ECurrencyCode.ARS => "AR",
            ECurrencyCode.ILS => "IL",
            ECurrencyCode.BYN => "BY",
            ECurrencyCode.KZT => "KZ",
            ECurrencyCode.KWD => "KW",
            ECurrencyCode.QAR => "QA",
            ECurrencyCode.CRC => "CT",
            ECurrencyCode.UYU => "UY",
            ECurrencyCode.BGN => "BG",
            ECurrencyCode.HRK => "HR",
            ECurrencyCode.CZK => "CZ",
            ECurrencyCode.DKK => "DK",
            ECurrencyCode.HUF => "HU",
            ECurrencyCode.RON => "RO",
            _ => "",
        };
    }

    /// <summary>
    /// 获取游戏数量
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    public static async Task<int> GetBotGameCount(Bot bot)
    {
        var request = new Uri(SteamCommunityURL, $"/profiles/{bot.SteamID}/badges/13?l=schinese");
        var response = await bot.ArchiWebHandler.UrlGetToHtmlDocumentWithSession(request, referer: SteamCommunityURL).ConfigureAwait(false);

        if (response?.Content == null)
        {
            return -1;
        }

        var eleDescription = response.Content.QuerySelector("div.badge_description");
        if (eleDescription == null)
        {
            return -1;
        }

        var match = RegexUtils.MatchGameCount().Match(eleDescription.TextContent);
        if (match.Success && int.TryParse(match.Groups[1].Value, out var gameCount))
        {
            return gameCount;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// 获取等级
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    public static async Task<int> GetBotLevel(Bot bot)
    {
        var request = new Uri(SteamCommunityURL, $"/profiles/{bot.SteamID}/badges?l=schinese");
        var response = await bot.ArchiWebHandler.UrlGetToHtmlDocumentWithSession(request, referer: SteamCommunityURL).ConfigureAwait(false);

        if (response?.Content == null)
        {
            return -1;
        }

        var eleDescription = response.Content.QuerySelector("span.friendPlayerLevelNum");
        if (eleDescription == null)
        {
            return -1;
        }

        if (int.TryParse(eleDescription.TextContent, out var gameCount))
        {
            return gameCount;
        }
        else
        {
            return 0;
        }
    }
}
