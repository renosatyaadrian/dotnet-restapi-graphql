using System;
using System.Net;
using System.Threading.Tasks;
using Confluent.Kafka;
using TwittorAPI.Constants;

namespace TwittorAPI.Kafka
{
    public class KafkaHelper
    {
        public static async Task<TransactionStatus> SendKafkaAsync(KafkaSettings settings,string topic,string key, string val)
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

            var ret = new TransactionStatus(succeed, "Success");
            if (!succeed)
                ret = new TransactionStatus(succeed, "Failed to submit data");
            
            return await Task.FromResult(ret);
        }
    }
}