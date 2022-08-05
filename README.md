# TrxSlackBot

Uses TRX test output file, takes total , passed, skipped, failed test numbers, calculates the urgency of fails (65% pass only warning, below critical) into slack mood colors and sending simple infos back to a SlackChannel via WebHook

### Config Json Setup

Use the embedded Config **slackAndTrxConfig.json** if you only plan to use it for one test result

For several test result setups use the config json filepath in the command argument
+ ex. <code>.\TrxSlackBot.exe 'C:\temp\TrxSlackBotProjectA.json'</code>
+ ex. <code>.\TrxSlackBot.exe 'C:\temp\TrxSlackBotProjectB.json'</code>

### Config Structure ###

+ slackWebhook
+ TrxFile location (ex. C:\temp\TestResults.trx)
+ Link to Detailed Test Results (ex. TestRail, Allure Report or GitHub Pages etc. )

```json
{
  "SlackAndTrxConfig": {
    "slackWebhook": "",
    "trxFile": "",
    "detailsLink": "" 
  }
}
```
