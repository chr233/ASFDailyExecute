# ASFAddFreeGames

| Command                        | Shortcut | Right           | Description                                           |
| ------------------------------ | -------- | --------------- | ----------------------------------------------------- |
| `ASFADDFREEGAMES`              | `AAFG`   | `FamilySharing` | Test plugin                                           |
| -                              | -        | -               | -                                                     |
| `RELOADFREEGAMES`              | `RFG`    | `Operator`      | Reload free game list from `plugins/FreeGameData.txt` |
| `FREEGAMESTATUS [Bots]`        | `FGS`    | `Operator`      | Get bot status about adding games                     |
| `ADDFREEGAME [Bots] Amount`    | `AFG`    | `Operator`      | Add specify amount of games to bot's library          |
| `PLAYRANDOMGAME [Bots] Amount` | `PRG`    | `Operator`      | Play random games in bot's library                    |
| `STOPPLAYGAME [Bots]`          | `SPG`    | `Operator`      | Stop playing random games                             |
| `SETPLAYTIME [Bots] Amount`    | `SPT`    | `Operator`      | Set playtime range for `PLAYRANDOMGAME`               |
| `WHATGAME [Bots]`              | `WG`     | `Operator`      | Get the name of the game, bot is current playing      |

```Plain
ADDFREEGAME Example:

AFG ASF 50 : add 50 games to bot's library
AFG ASF 30-80 : add random (from 30 to 80) games to bot's library
```

---

```Plain
PLAYRANDOMGAME Example:

PRG ASF 5 : play random 5 games from bot's library
PRG ASF 10-30 : play random (from 10 to 30) games from bot's library

*bot can play up to 32 games at the same time*
```

---

```Plain
SETPLAYTIME Example:

SPT ASF 0 : set playtime to unlimited
SPT ASF 5 : set playtime to 5 minutes
SPT ASF 10-300 : set playtime to from 10 to 300 minutes (every game will use a random time)

*playtime setting will be stored*
```
