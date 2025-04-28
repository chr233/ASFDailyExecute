# ASFDailyExecute

## 配置文件

```json
{
  "IPCPassword": "",
  "CurrentCulture": "zh-CN",
  "InventoryLimiterDelay": 5,
  "MaxTradeHoldDuration": 1,
  "SteamMessagePrefix": null,
  "UpdatePeriod": 0,
  "Headless": true,
  "ASFEnhance": {
    "EULA": true,
    "Statistic": true,
    "ExecuteTime": "00:00",
    "OfflineAfterExecute": false
  }
}
```

| 配置项目              | 类型   | 默认值  | 说明                                                                              |
| --------------------- | ------ | ------- | --------------------------------------------------------------------------------- |
| `EULA`                | `bool` | `false` | 是否同意使用协议                                                                  |
| `Statistic`           | `bool` | `true`  | 发送统计信息                                                                      |
| `ExecuteTime`         | `str`  | `00:00` | 脚本每日执行时间                                                                  |
| `OfflineAfterExecute` | `bool` | `true`  | 脚本执行结束以后是否离线机器人 (执行脚本前就在线的, 或者正在挂卡的机器人不受影响) |

## 命令

| Command           | Shortcut | Right           | Description  |
| ----------------- | -------- | --------------- | ------------ |
| `ASFDailyExecute` | `ADE`    | `FamilySharing` | 测试插件     |
| -                 | -        | -               | -            |
| `GETSCRIPT`       | `GS`     | `Operator`      | 查看脚本内容 |
| `RESETSCRIPT`     | `RS`     | `Operator`      | 重置脚本内容 |

脚本位于 `pugins/ASFDailyExecute.txt`

示例脚本

```plain
#
# 脚本说明 by chr_
#
# 1. # 后面的内容会被忽略
# 2. $ 会被替换为机器人名
# 3. 脚本一行一句

LEVEL $
BALANCE $
STATUS $
```
