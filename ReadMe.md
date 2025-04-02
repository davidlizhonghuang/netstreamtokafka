# .Net data streamer to pull data from Kafka

To pull data from Kafka in a .NET application, you can use the `Confluent.Kafka` library, which is a popular library for working with Kafka in .NET. Below is an example of how you can create a data streamer to consume messages from a Kafka topic.

### Step 1: Install the Confluent.Kafka NuGet Package
First, you need to install the `Confluent.Kafka` NuGet package. You can do this using the NuGet Package Manager or by running the following command in the Package Manager Console:
 
 dotnet add package Confluent.Kafka

### Step 2: Create the Kafka Consumer
Here is an example of how to create a Kafka consumer in a .NET application:

    using System;
    using System.Threading;
    using Confluent.Kafka;

    class Program { 
        static void Main(string[] args) {

                var config = new ConsumerConfig {
                    BootstrapServers = "localhost:9092",
                    GroupId = "my-consumer-group",
                    AutoOffsetReset = AutoOffsetReset.Earliest
                    };

                using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build()) {
                    consumer.Subscribe("my-topic");
                    var cts = new CancellationTokenSource();
                    Console.CancelKeyPress += (_, e) => {
                        e.Cancel = true;
                        cts.Cancel();
                        };
                        try {
                                while (true) {
                                    var cr = consumer.Consume(cts.Token);
                                    Console.WriteLine($"Consumed message '{cr.Message.Value}' at: '{cr.TopicPartitionOffset}'.");
                                    }
                                    }
                                    catch (OperationCanceledException) {
                                        consumer.Close();
                                        }
                                        }
                                        }
                                        }

### Step 3: Run the Consumer
Run the application, and it will start consuming messages from the Kafka topic. The consumer will continue to run until you press Ctrl+C, at which point it will gracefully shut down.  

### Step 4: Handle Deserialization (Optional)
If your Kafka messages are not in string format, you will need to handle deserialization. The `Confluent.Kafka` library allows you to specify custom deserializers for both the key and value of the Kafka message.

    using Confluent.Kafka;
    using System;
    using System.Threading;
    class Program {
        static void Main(string[] args) {
            var config = new ConsumerConfig {
                BootstrapServers = "localhost:9092",
                GroupId = "my-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
                };

                        using (var consumer = new ConsumerBuilder<Ignore, MyMessageType>(config)
                        .SetValueDeserializer(new MyMessageTypeDeserializer())
                        .Build()
                            ) {
                                    consumer.Subscribe("my-topic");
                                    var cts = new CancellationTokenSource();
                                    Console.CancelKeyPress += (_, e) => {
                                        e.Cancel = true;
                                        cts.Cancel();
                                        };
                                        try {
                                                while (true) {
                                                    var cr = consumer.Consume(cts.Token);
                                                    Console.WriteLine($"Consumed message '{cr.Message.Value}' at: '{cr.TopicPartitionOffset}'.");
                                                    }

                                                    }
                                                    catch (OperationCanceledException) {
                                                        consumer.Close();
                                                        }
                                                        }
                                                        }
  










