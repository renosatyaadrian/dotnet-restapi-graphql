using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Configuration;

namespace KafkaListeningApp
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                    .AddJsonFile($"appsettings.json", true, true);

            var config = builder.Build();
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = "0.0.0.0:9092",
                ClientId = Dns.GetHostName(),
            };
            var topics = new List<String>();
            topics.Add("order-add");
            foreach(var topic in topics)
            {
                using (var adminClient = new AdminClientBuilder(producerConfig).Build())
                {
                    Console.WriteLine("Creating a topic....");
                    try
                    {
                        await adminClient.CreateTopicsAsync(new List<TopicSpecification> {
                        new TopicSpecification { Name = topic, NumPartitions = 1, ReplicationFactor = 1 } });
                    }
                    catch (CreateTopicsException e)
                    {
                        if (e.Results[0].Error.Code != ErrorCode.TopicAlreadyExists)
                        {
                            Console.WriteLine($"An error occured creating topic {topic}: {e.Results[0].Error.Reason}");
                        }
                        else
                        {
                            Console.WriteLine("Topic already exists");
                        }
                    }
                }
            }
            // var Serverconfig = new ConsumerConfig
            // {
            //     BootstrapServers = "localhost:9092",
            //     GroupId = "logging",
            //     AutoOffsetReset = AutoOffsetReset.Latest
            // };
            // CancellationTokenSource cts = new CancellationTokenSource();
            // Console.CancelKeyPress += (_, e) => {
            //     e.Cancel = true; // prevent the process from terminating.
            //     cts.Cancel();
            // };

            // using (var consumer = new ConsumerBuilder<string, string>(Serverconfig).Build())
            // {
            //     Console.WriteLine("Connected");
            //     consumer.Subscribe("logging");
            //     Console.WriteLine("Waiting messages....");
            //     try
            //     {
            //         while (true)
            //         {
            //             var cr = consumer.Consume(cts.Token);
            //             Console.WriteLine($"Consumed record with key: {cr.Message.Key} and value: {cr.Message.Value}");
            //         }
            //     }
            //     catch (OperationCanceledException)
            //     {
            //         // Ctrl-C was pressed.
            //     }
            //     finally
            //     {
            //         consumer.Close();
            //     }
            // }
            return 0;
        }
    }
}