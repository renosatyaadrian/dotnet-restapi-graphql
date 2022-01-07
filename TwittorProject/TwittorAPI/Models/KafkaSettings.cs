using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwittorAPI.Models
{
    public class KafkaSettings
    {
        public string Server { get; set; }
        public int NumPartitions { get; set; }
        public short ReplicationFactor { get; set; }
    }
}