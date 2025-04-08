using ArchiSteamFarm.Steam;

namespace ASFDailyExecute;
/// <summary>
/// AccessToken 为NULL
/// </summary>
public class AccessTokenNullException(Bot bot) : Exception(bot.BotName)
{
}
