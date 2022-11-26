using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet;
using MQTTnet.Extensions.Rpc;
using Net.Utilities.Object.String.Constant;
using Net.Utilities.Struct.Byte;

var mqttFactory = new MqttFactory();

using var mqttClient = mqttFactory.CreateMqttClient();
var mqttClientOptions = new MqttClientOptionsBuilder()
    .WithClientId(Guid.NewGuid().ToString())
    .WithTcpServer("iot.nnmz.aacoptics.com", 1883)
    .WithCredentials("admin", "public")
    .WithTimeout(TimeSpan.FromSeconds(30))
    .WithKeepAlivePeriod(TimeSpan.FromSeconds(20))
    .Build();

await mqttClient.ConnectAsync(mqttClientOptions);

using var mqttRpcClient = mqttFactory.CreateMqttRpcClient(mqttClient);

while (true)
{
    var randomGuidString = EnvironmentHelper.RandomGuidString;
    var executeAsync =
        await mqttRpcClient.ExecuteAsync(TimeSpan.FromSeconds(2), "ping", randomGuidString,
            MqttQualityOfServiceLevel.AtMostOnce);
    var bytes2Utf8String = ByteCastHelper.Bytes2Utf8String(executeAsync);
    Console.WriteLine(bytes2Utf8String);
    if (bytes2Utf8String != randomGuidString)
    {
        throw new Exception("测试失败");
    }
}