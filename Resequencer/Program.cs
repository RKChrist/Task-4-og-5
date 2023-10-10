using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory
{
    HostName = Environment.GetEnvironmentVariable("HostName") ?? "rabbitmq",
    Port = Environment.GetEnvironmentVariable("Port") != default ? int.Parse(Environment.GetEnvironmentVariable("Port")) : 5672,
    UserName = Environment.GetEnvironmentVariable("UserName") ?? "guest",
    Password = Environment.GetEnvironmentVariable("UserName") ?? "guest"
};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

var consumer = new EventingBasicConsumer(channel);

channel.QueueDeclare(queue: "consumeProducer",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

consumer.Received += (model, ea) => {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

         channel.BasicPublish(exchange: "random",
                                         routingKey: string.Empty,
                                         basicProperties: null,
                                         body: body);
};

channel.BasicConsume(queue: "EtEllerAndet",
                    autoAck: true,
                    consumer: consumer);


Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();