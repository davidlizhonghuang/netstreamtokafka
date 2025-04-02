
internal class Program
{
    private static async Task Main(string[] args)
    {
        var kafkaConsumer = new KafkaConsumerWithChannel();
        Console.CancelKeyPress += (sender, e) =>
        {
            kafkaConsumer.Stop();
            Console.WriteLine("Kafka Consumer Stopped.");
        };

        await kafkaConsumer.StartAsync();
    }
}