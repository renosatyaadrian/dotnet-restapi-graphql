using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using TwittorAPI.GraphQL;
using TwittorAPI.Models;

namespace TwittorAPI.Kafka
{
    public class KafkaHelper
    {
        public static async Task<bool> SendMessage(KafkaSettings settings,string topic,string key, string val)
        {
            var succeed = false;
            var config = new ProducerConfig
            {
                BootstrapServers = settings.Server,
                ClientId = Dns.GetHostName(),

            }; 
            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {
                producer.Produce(topic, new Message<string, string>
                {
                    Key = key,
                    Value = val
                }, (deliveryReport) =>
                {
                    if (deliveryReport.Error.Code != ErrorCode.NoError)
                    {
                        Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                    }
                    else
                    {
                        Console.WriteLine($"Produced message to: {deliveryReport.TopicPartitionOffset}");
                        succeed = true;
                    }
                });
                producer.Flush(TimeSpan.FromSeconds(10));
            }
            return await Task.FromResult(succeed);
        }

        public static async Task<TransactionStatus> SendKafkaAsync(KafkaSettings kafkaSettings, string topic, string key, string val)
        {
            var result = await KafkaHelper.SendMessage(kafkaSettings, topic, key,val);
            await KafkaHelper.SendMessage(kafkaSettings, "logging", key, val);
            var ret = new TransactionStatus(result, "");
            if (!result)
                ret = new TransactionStatus(result, "Failed to submit data");
            
            return await Task.FromResult(ret);
        }
    }
}