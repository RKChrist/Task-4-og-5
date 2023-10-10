using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory
{
    HostName = "localhost",
    Port = 5672,
    UserName = "guest",
    Password = "guest"
};

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

var consumer = new EventingBasicConsumer(channel);
            
channel.QueueDeclare(queue: "consumeProducer",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);


Model m = new();
consumer.Received += (model, ea) => {

        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        
        m.Messages.Add(new Message{
            Body= message,
            Id = m.Messages.Count});
        
        //Resequencer message logic
        m.Messages.OrderBy(s => s.Id);

        System.Console.WriteLine(JsonConvert.SerializeObject(m));   

        bool finalmsg = (bool)ea.BasicProperties.Headers["Final"];
        System.Console.WriteLine("\nPrint:" + finalmsg);
        if(finalmsg){
            System.Console.WriteLine("Final message received");
            System.Console.WriteLine(JsonConvert.SerializeObject(m));
            m.Messages.Clear();
        }


        // Does something really cool with the message
        var jsonBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(m)); 

        // Something really cool :)
};


channel.BasicConsume(queue: "consumeProducer",
                                 autoAck: true,
                                 consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();


// {
//    "body": "hey",
//    "id": 6
// }