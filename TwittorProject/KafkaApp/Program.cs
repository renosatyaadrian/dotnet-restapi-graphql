using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaApp.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace KafkaApp
{
    class Program
    {
        static async Task<int>  Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                    .AddJsonFile($"appsettings.json", true, true);

            var config = builder.Build();


            var Serverconfig = new ConsumerConfig
            {
                BootstrapServers = config["Settings:KafkaServer"],
                GroupId = "tester",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => {
                e.Cancel = true; // prevent the process from terminating.
                cts.Cancel();
            };
            Console.WriteLine("--------------.NET Application--------------");
            using (var consumer = new ConsumerBuilder<string, string>(Serverconfig).Build())
            {
                Console.WriteLine("Connected");
                var topics = new string[] { 
                    "user-add", 
                    "user-update",
                    "role-add", 
                    "user-role-add", 
                    "twittor-add", 
                    "twittor-delete", 
                    "comment-add"  
                };
                consumer.Subscribe(topics);

                Console.WriteLine("Waiting messages....");
                try
                {
                    while (true)
                    {
                        var cr = consumer.Consume(cts.Token);
                        Console.WriteLine($"Consumed record with Topic: {cr.Topic} key: {cr.Message.Key} and value: {cr.Message.Value}");

                        using (var dbcontext = new twittorsContext())
                        {
                            switch(cr.Topic)
                            {
                                case "user-add":
                                User user = JsonConvert.DeserializeObject<User>(cr.Message.Value);
                                dbcontext.Users.Add(user);
                                break;
                                case "user-update":
                                User userUpdate = JsonConvert.DeserializeObject<User>(cr.Message.Value);
                                dbcontext.Users.Update(userUpdate);
                                break;
                                case "role-add":
                                Role role = JsonConvert.DeserializeObject<Role>(cr.Message.Value);
                                dbcontext.Roles.Add(role);
                                break;
                                case "user-role-add":
                                UserRole userRole = JsonConvert.DeserializeObject<UserRole>(cr.Message.Value);
                                dbcontext.UserRoles.Add(userRole);
                                break;
                                case "twittor-add":
                                Twittor twittor = JsonConvert.DeserializeObject<Twittor>(cr.Message.Value);
                                dbcontext.Twittors.Add(twittor);
                                break;
                                case "twittor-delete":
                                Twittor twittorDelete = JsonConvert.DeserializeObject<Twittor>(cr.Message.Value);
                                dbcontext.Twittors.Remove(twittorDelete);
                                break;
                                case "comment-add":
                                Comment comment = JsonConvert.DeserializeObject<Comment>(cr.Message.Value);
                                dbcontext.Comments.Add(comment);
                                break;
                            }
                            
                            await dbcontext.SaveChangesAsync();
                            Console.WriteLine("Data was saved into database");
                        }
                        
                        
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ctrl-C was pressed.
                }
                finally
                {
                    consumer.Close();
                }

            }

            return 1;
        }
    }
}
