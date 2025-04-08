using ArchiSteamFarm.Core;
using ArchiSteamFarm.NLog;
using ArchiSteamFarm.Steam;
using ArchiSteamFarm.Steam.Integration;
using ArchiSteamFarm.Web;
using ArchiSteamFarm.Web.Responses;
using ASFDailyExecute.Core;
using ASFDailyExecute.Data;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using static ArchiSteamFarm.Steam.Integration.ArchiWebHandler;

namespace ASFDailyExecute;

internal static class Utils
{
    internal const StringSplitOptions SplitOptions =
        StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

    /// <summary>
    ///     逗号分隔符
    /// </summary>
    internal static readonly char[] SeparatorDot = [','];

    /// <summary>
    ///     加号分隔符
    /// </summary>
    internal static readonly char[] SeparatorPlus = ['+'];

    /// <summary>
    ///     逗号空格分隔符
    /// </summary>
    internal static readonly char[] SeparatorDotSpace = [',', ' '];

    internal static readonly char[] NewLineSeperator = ['\r', '\n'];
    internal static ScriptManager ScriptMgr { get; } = new();

    /// <summary>
    ///     插件配置
    /// </summary>
    internal static PluginConfig Config { get; set; } = null!;

    internal static ConcurrentDictionary<Bot, string?> CustomUserCountry { get; } = [];

    /// <summary>
    ///     获取版本号
    /// </summary>
    internal static Version MyVersion => Assembly.GetExecutingAssembly().GetName().Version ?? new Version("0.0.0.0");

    /// <summary>
    ///     获取ASF版本
    /// </summary>
    internal static Version ASFVersion => typeof(ASF).Assembly.GetName().Version ?? new Version("0.0.0.0");

    /// <summary>
    ///     获取插件所在路径
    /// </summary>
    internal static string MyLocation => Assembly.GetExecutingAssembly().Location;

    /// <summary>
    ///     获取插件所在文件夹路径
    /// </summary>
    internal static string MyDirectory => Path.GetDirectoryName(MyLocation) ?? ".";

    /// <summary>
    ///     Steam商店链接
    /// </summary>
    internal static Uri SteamStoreURL => ArchiWebHandler.SteamStoreURL;

    /// <summary>
    ///     Steam社区链接
    /// </summary>
    internal static Uri SteamCommunityURL => ArchiWebHandler.SteamCommunityURL;

    /// <summary>
    ///     SteamAPI链接
    /// </summary>
    internal static Uri SteamApiURL => new("https://api.steampowered.com");

    /// <summary>
    ///     Steam结算链接
    /// </summary>
    internal static Uri SteamCheckoutURL => ArchiWebHandler.SteamCheckoutURL;

    internal static Uri SteamHelpURL => ArchiWebHandler.SteamHelpURL;

    /// <summary>
    ///     日志
    /// </summary>
    internal static ArchiLogger ASFLogger => ASF.ArchiLogger;

    /// <summary>
    ///     格式化返回文本
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    internal static string FormatStaticResponse(string message) => $"<ASFE> {message}";

    /// <summary>
    ///     格式化返回文本
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    internal static string FormatStaticResponse(string message, params object?[] args) =>
        FormatStaticResponse(string.Format(message, args));

    /// <summary>
    ///     格式化返回文本
    /// </summary>
    /// <param name="bot"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    internal static string FormatBotResponse(this Bot bot, string message) => $"<{bot.BotName}> {message}";

    /// <summary>
    ///     格式化返回文本
    /// </summary>
    /// <param name="bot"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    internal static string FormatBotResponse(this Bot bot, string message, params object?[] args) =>
        bot.FormatBotResponse(string.Format(message, args));

    internal static StringBuilder AppendLineFormat(this StringBuilder sb, string format, params object?[] args) =>
        sb.AppendLine(string.Format(format, args));

    /// <summary>
    ///     获取个人资料链接
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    internal static async Task<string?> GetProfileLink(this Bot bot) =>
        await bot.ArchiWebHandler.GetAbsoluteProfileURL().ConfigureAwait(false);

    /// <summary>
    ///     转换SteamId
    /// </summary>
    /// <param name="steamId"></param>
    /// <returns></returns>
    internal static ulong SteamId2Steam32(ulong steamId) =>
        IsSteam32ID(steamId) ? steamId : steamId - 0x110000100000000;

    /// <summary>
    ///     转换SteamId
    /// </summary>
    /// <param name="steamId"></param>
    /// <returns></returns>
    internal static ulong Steam322SteamId(ulong steamId) =>
        IsSteam32ID(steamId) ? steamId + 0x110000100000000 : steamId;

    internal static bool IsSteam32ID(ulong id) => id <= 0xFFFFFFFF;

    /// <summary>
    ///     布尔转换为char
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    internal static char Bool2Str(bool b) => b ? '√' : '×';

    internal static char ToStr(this bool b) => Bool2Str(b);

    /// <summary>
    ///     跳过参数获取Bot名称
    /// </summary>
    /// <param name="args"></param>
    /// <param name="skipStart"></param>
    /// <param name="skipEnd"></param>
    /// <returns></returns>
    internal static string SkipBotNames(string[] args, int skipStart, int skipEnd) =>
        string.Join(',', args[skipStart..(args.Length - skipEnd)]);

    internal static Task<ObjectResponse<T>?> UrlPostToJsonObject<T>(this ArchiWebHandler handler,
        Uri request,
        IDictionary<string, string>? data = null,
        Uri? referer = null,
        WebBrowser.ERequestOptions requestOptions = WebBrowser.ERequestOptions.None,
        bool checkSessionPreemptively = true,
        byte maxTries = WebBrowser.MaxTries,
        int rateLimitingDelay = 0,
        bool allowSessionRefresh = true,
        CancellationToken cancellationToken = default) =>
        handler.UrlPostToJsonObjectWithSession<T>(request, null, data, referer, requestOptions, ESession.None,
            checkSessionPreemptively, maxTries, rateLimitingDelay, allowSessionRefresh, cancellationToken);

    internal static Task<HtmlDocumentResponse?> UrlPostToHtmlDocument(this ArchiWebHandler handler,
        Uri request,
        IDictionary<string, string>? data = null,
        Uri? referer = null,
        WebBrowser.ERequestOptions requestOptions = WebBrowser.ERequestOptions.None,
        bool checkSessionPreemptively = true,
        byte maxTries = WebBrowser.MaxTries,
        int rateLimitingDelay = 0,
        bool allowSessionRefresh = true,
        CancellationToken cancellationToken = default) =>
        handler.UrlPostToHtmlDocumentWithSession(request, null, data, referer, requestOptions, ESession.None,
            checkSessionPreemptively, maxTries, rateLimitingDelay, allowSessionRefresh, cancellationToken);

    internal static Task<bool> UrlPost(this ArchiWebHandler handler,
        Uri request,
        IDictionary<string, string>? data = null,
        Uri? referer = null,
        WebBrowser.ERequestOptions requestOptions = WebBrowser.ERequestOptions.None,
        bool checkSessionPreemptively = true,
        byte maxTries = WebBrowser.MaxTries,
        int rateLimitingDelay = 0,
        bool allowSessionRefresh = true,
        CancellationToken cancellationToken = default) =>
        handler.UrlPostWithSession(request, null, data, referer, requestOptions, ESession.None,
            checkSessionPreemptively, maxTries, rateLimitingDelay, allowSessionRefresh, cancellationToken);

    internal static async Task SaveToFile(string fileName, string fileContent)
    {
        var filePath = Path.Combine(MyDirectory, $"{fileName}.txt");

        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using var file = File.CreateText(filePath);
            await file.WriteAsync(fileContent).ConfigureAwait(false);
            await file.FlushAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            ASFLogger.LogGenericError(string.Format("写入文件至 {0} 失败", filePath));
            ASFLogger.LogGenericException(ex);
        }
    }
}