using System.Threading.Channels;
using Confluent.Kafka;

public class KafkaConsumerWithChannel
{
    private const string KafkaBootstrapServers = "172.21.68.59:9092";
    private const string KafkaTopic = "fir-topic";
    private readonly Channel<string> _channel;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public KafkaConsumerWithChannel()
    {
        _channel = Channel.CreateBounded<string>(new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait // Wait if full (backpressure handling)
        });

        _cancellationTokenSource = new CancellationTokenSource();
    }

    public async Task StartAsync()
    {
        var consumerTask = Task.Run(() => ConsumeKafkaMessages(_cancellationTokenSource.Token));
        var processingTask = Task.Run(() => ProcessMessages(_cancellationTokenSource.Token));

        await Task.WhenAll(consumerTask, processingTask);
    }

    private async Task ConsumeKafkaMessages(CancellationToken cancellationToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = KafkaBootstrapServers,
            GroupId = "my-fi-app",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(KafkaTopic);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(cancellationToken);
                    Console.WriteLine($"Kafka Received: {consumeResult.Message.Value}");

                    // Add to channel (wait if full)
                    await _channel.Writer.WriteAsync(consumeResult.Message.Value, cancellationToken);
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex)
                {
                    Console.WriteLine($"Kafka Consumer Error: {ex.Message}");
                }
            }
        }
        finally
        {
            consumer.Close();
        }
    }

    private async Task ProcessMessages(CancellationToken cancellationToken)
    {
        while (await _channel.Reader.WaitToReadAsync(cancellationToken))
        {
            while (_channel.Reader.TryRead(out var message))
            {
                Console.WriteLine($"Processing Message: {message}");
                await Task.Delay(500); // Simulate processing time
            }
        }
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
    }
}
