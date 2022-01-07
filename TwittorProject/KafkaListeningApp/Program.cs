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
        {var config = new ProducerConfig
            {
                BootstrapServers = "127.0.0.1:9092",
                ClientId = Dns.GetHostName(),

            };
            var topics = new List<String>();
            topics.Add("logging");
            topics.Add("role");
            topics.Add("user");
            foreach(var topic in topics)
            {
                using (var adminClient = new AdminClientBuilder(config).Build())
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

            return 0;
        }
    }
}
