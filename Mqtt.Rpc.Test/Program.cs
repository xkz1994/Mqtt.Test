using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet;

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


await mqttClient.SubscribeAsync("MQTTnet.RPC/+/ping");

mqttClient.ApplicationMessageReceivedAsync += eventArgs =>
{
    Console.WriteLine(
        $"{eventArgs.ApplicationMessage.Topic} => {eventArgs.ApplicationMessage.ConvertPayloadToString()}");
    if (eventArgs.ApplicationMessage.Topic.StartsWith("MQTTnet.RPC/"))
    {
        // ReSharper disable once AccessToDisposedClosure
        return mqttClient.PublishAsync(new MqttApplicationMessage
        {
            Topic = $"{eventArgs.ApplicationMessage.Topic}/response",
            QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce,
            Payload = eventArgs.ApplicationMessage.Payload
        });
    }

    return Task.CompletedTask;
};

Console.ReadKey();