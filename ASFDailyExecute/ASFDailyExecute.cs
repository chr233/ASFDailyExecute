using ArchiSteamFarm.Core;
using ArchiSteamFarm.Helpers.Json;
using ArchiSteamFarm.Plugins.Interfaces;
using ArchiSteamFarm.Steam;
using ASFDailyExecute.Core;
using ASFDailyExecute.Data;
using System.ComponentModel;
using System.Composition;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace ASFDailyExecute;

[Export(typeof(IPlugin))]
internal sealed class ASFDailyExecute : IASF, IBotCommand2
{
    private bool ASFEBridge;

    private Timer? StatisticTimer;

    /// <summary>
    ///     获取插件信息
    /// </summary>
    private string PluginInfo => $"{Name} {Version}";

    public string Name => "ASF Daily Execute";

    public Version Version => MyVersion;

    /// <summary>
    ///     ASF启动事件
    /// </summary>
    /// <param name="additionalConfigProperties"></param>
    /// <returns></returns>
    public Task OnASFInit(IReadOnlyDictionary<string, JsonElement>? additionalConfigProperties = null)
    {
        PluginConfig? config = null;

        if (additionalConfigProperties != null)
        {
            foreach (var (configProperty, configValue) in additionalConfigProperties)
            {
                if (configProperty != "ASFEnhance" || configValue.ValueKind != JsonValueKind.Object)
                {
                    continue;
                }

                try
                {
                    config = configValue.ToJsonObject<PluginConfig>();
                    if (config != null)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    ASFLogger.LogGenericException(ex);
                }
            }
        }

        Config = config ?? new PluginConfig(false, false);

        var sb = new StringBuilder();

        //使用协议
        if (!Config.EULA)
        {
            sb.AppendLine();
            sb.AppendLine(Langs.Line);
            sb.AppendLineFormat(Langs.EulaWarning, Name);
            sb.AppendLine(Langs.Line);
        }

        if (sb.Length > 0)
        {
            ASFLogger.LogGenericWarning(sb.ToString());
        }

        //统计
        if (Config.Statistic && !ASFEBridge)
        {
            var request = new Uri("https://asfe.chrxw.com/asfdailyexecute");
            StatisticTimer = new Timer(
                async _ => await ASF.WebBrowser!.UrlGetToHtmlDocument(request).ConfigureAwait(false),
                null,
                TimeSpan.FromSeconds(30),
                TimeSpan.FromHours(24)
            );
        }

        if (Config.EULA)
        {
            ScriptManager.Init();
        }

        return Task.CompletedTask;
    }

    /// <summary>
    ///     插件加载事件
    /// </summary>
    /// <returns></returns>
    public Task OnLoaded()
    {
        ASFLogger.LogGenericInfo(Langs.PluginContact);
        ASFLogger.LogGenericInfo(Langs.PluginInfo);

        const BindingFlags flag = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        var handler = typeof(ASFDailyExecute).GetMethod(nameof(ResponseCommand), flag);

        const string pluginId = nameof(ASFDailyExecute);
        const string cmdPrefix = "ADE";
        const string repoName = "ASFDailyExecute";

        ASFEBridge = AdapterBridge.InitAdapter(Name, pluginId, cmdPrefix, repoName, handler);

        if (ASFEBridge)
        {
            ASFLogger.LogGenericDebug(Langs.ASFEnhanceRegisterSuccess);
        }
        else
        {
            ASFLogger.LogGenericInfo(Langs.ASFEnhanceRegisterFailed);
            ASFLogger.LogGenericWarning(Langs.PluginStandalongMode);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    ///     处理命令事件
    /// </summary>
    /// <param name="bot"></param>
    /// <param name="access"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    /// <param name="steamId"></param>
    /// <returns></returns>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<string?> OnBotCommand(Bot bot, EAccess access, string message, string[] args, ulong steamId = 0)
    {
        if (ASFEBridge)
        {
            return null;
        }

        if (!Enum.IsDefined(access))
        {
            throw new InvalidEnumArgumentException(nameof(access), (int)access, typeof(EAccess));
        }

        try
        {
            var cmd = args[0].ToUpperInvariant();

            if (cmd.StartsWith("ADE."))
            {
                cmd = cmd[4..];
            }

            var task = ResponseCommand(bot, access, cmd, args);
            if (task != null)
            {
                return await task.ConfigureAwait(false);
            }

            return null;
        }
        catch (Exception ex)
        {
            _ = Task.Run(async () => {
                await Task.Delay(500).ConfigureAwait(false);
                ASFLogger.LogGenericException(ex);
            }).ConfigureAwait(false);

            return ex.StackTrace;
        }
    }

    /// <summary>
    ///     处理命令
    /// </summary>
    /// <param name="bot"></param>
    /// <param name="access"></param>
    /// <param name="cmd"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private Task<string>? ResponseCommand(Bot bot, EAccess access, string cmd, string[] args)
    {
        var argLength = args.Length;

        return argLength switch {
            0 => throw new InvalidOperationException(nameof(args)),
            1 => cmd switch //不带参数
            {
                //插件信息
                "ASFDAILYEXECUTE" or
                "ADE" when access >= EAccess.FamilySharing => Task.FromResult(PluginInfo),

                "GETSCRIPT" or
                "GS" when access >= EAccess.Operator => Command.ResponseGetScript(),

                "RESETSCRIPT" or
                "RS" when access >= EAccess.Operator => Command.ResponseResetScript(),

                _ => null
            },
            _ => cmd switch //带参数
            {
                "GETSCRIPT" or
                "GS" when access >= EAccess.Operator => Command.ResponseGetScript(),

                "RESETSCRIPT" or
                "RS" when access >= EAccess.Operator => Command.ResponseResetScript(),

                _ => null
            }
        };
    }
}