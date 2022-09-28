# TrxSlackBot

Uses TRX test output file, takes total , passed, skipped, failed test numbers, calculates the urgency of fails (65% pass only warning, below critical) into slack mood colors and sending simple infos back to a SlackChannel via WebHook

### Config Json Setup

Use the embedded Config **slackAndTrxConfig.json** if you only plan to use it for one test result

For several test result setups use the config json filepath in the command argument
+ ex. <code>.\TrxSlackBot.exe 'C:\temp\TrxSlackBotProjectA.json'</code>
+ ex. <code>.\TrxSlackBot.exe 'C:\temp\TrxSlackBotProjectB.json'</code>

### Config Structure

+ slackWebhook
+ healthCheckWebhook
+ slackBearerToken (for slack api use)
+ trxFile - location of test result file (ex. C:\temp\TestResults.trx)
+ detailsLink - Link to Detailed Test Results (ex. TestRail, Allure Report or GitHub Pages etc. )
+ configMessageTitle - (optional) Specific bot message title. Empty value defaults into configFileName without suffix
+ sendDetailedMessageAsReply
+ sendDetailedMessageAsReplyWaitSecondsForMessage
+ sendOnlyIfRunHasFails - bool (default - false)
+ sendFailsAsReply - bool (default - false)
+ channelId - Id of Slack Channel the slack reply message can be found
+ replyMessageTsId - (optional) Id of message that should receive a reply (this is received via slack channel history)
+ replyText - (optional) Specific Title for reply message
+ waitSecondsAfterMessageBeforeReply - seconds system should wait for reply message 
+ sendFailsAsCodeSnipped - bool (default - false)
+ snippedInitialComment - text snippet comment
+ snippedSlackFileName - text snippet file name
+ snippedSlackFilePostTitle - text snippet title
+ snippedSlackFileType": - type of file for code highlighting (default - xml)

### Config file Example

```json
{
  "TrxSlackBotConfig": {
    "slackWebhook": "",
    "healthCheckWebhook": "",
    "slackBearerToken": "",
    "trxFile": "",
    "detailsLink": "",
    "configMessageTitle": "",
    "sendDetailedMessageAsReply": false,
    "sendDetailedMessageAsReplyWaitSecondsForMessage": 0,
    "sendOnlyIfRunHasFails": false,
    "sendFailsAsReply": false,
    "channelId": "",
    "replyMessageTsId": "",
    "replyText": "",
    "waitSecondsAfterMessageBeforeReply": 1,
    "sendFailsAsCodeSnipped": false,
    "snippedInitialComment": "",
    "snippedSlackFileName": "",
    "snippedSlackFilePostTitle": "",
    "snippedSlackFileType": "xml"
  }
}
```

#### ToDo
This project was a means to an end but as I started developing new features were requested therefore the code requires a proper refactoring next especially in serialization, communication and the config file needs some structure.

Next on the list 
- Add options to send messages either via api or webhook
- Recognize and pull data from other test result output types
- Big method optimization
- Config File Refactoring
