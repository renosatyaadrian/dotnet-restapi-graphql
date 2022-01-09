using System;
using System.Collections.Generic;

#nullable disable

namespace KafkaApp.Models
{
    public partial class Comment
    {
        public int Id { get; set; }
        public string CommentDesc { get; set; }
        public int TwittorId { get; set; }

        public virtual Twittor Twittor { get; set; }
    }
}
