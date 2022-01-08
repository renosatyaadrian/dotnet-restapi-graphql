using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TwittorAPI.Input;
using TwittorAPI.Kafka;
using TwittorAPI.Models;
using HotChocolate.AspNetCore.Authorization;

namespace TwittorAPI.GraphQL
{
    public class TwittorMutation
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<KafkaSettings> _kafkaSettings;

        public TwittorMutation(IHttpContextAccessor httpContextAccessor, [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            _httpContextAccessor = httpContextAccessor;
            _kafkaSettings = kafkaSettings;
        }
        
        [Authorize(Roles = new [] { "User" })]
        public async Task<TransactionStatus> PostTwittorAsync([Service] AppDbContext context, CreateTwittorInput input)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst("Id").Value;
            var user = context.Users.Where(user=>user.Id == Convert.ToInt32(userId)).SingleOrDefault();

            var newTwit = new Twittor
            {
                Twit = input.Twittor,
                Created = DateTime.Now,
                UserId = Convert.ToInt32(userId)
            };
            var key = "twittor-add-" + DateTime.Now.ToString();
            var val = JObject.FromObject(newTwit).ToString(Formatting.None);
            string topic = "twittor";
            await KafkaHelper.SendKafkaAsync(_kafkaSettings.Value, "logging", key, val);
            return await KafkaHelper.SendKafkaAsync(_kafkaSettings.Value, topic, key, val);
        }

        [Authorize(Roles = new [] { "User" })]
        public async Task<TransactionStatus> DeleteTwittorAsync([Service] AppDbContext context, DeleteTwitInput input)
        {
            var twit = context.Twittors.Where(twit=>twit.Id==input.Id).SingleOrDefault();
            if(twit == null) return await Task.FromResult(new TransactionStatus(false, "Twit not found"));

            var key = "twittor-delete-" + DateTime.Now.ToString();
            var val = JObject.FromObject(twit).ToString(Formatting.None);
            string topic = "twittor";
            await KafkaHelper.SendKafkaAsync(_kafkaSettings.Value, "logging", key, val);
            return await KafkaHelper.SendKafkaAsync(_kafkaSettings.Value, topic, key, val);
        }


        [Authorize(Roles = new [] { "User" })]
        public async Task<TransactionStatus> CommentTwitAsync([Service] AppDbContext context, CommentTwitInput input)
        {
            var twit = context.Twittors.Where(twit=>twit.Id==input.TwitorId).SingleOrDefault();
            if(twit == null) return await Task.FromResult(new TransactionStatus(false, "Twit not found"));

            var comment = new Comment
            {
                CommentDesc = input.Comment,
                TwittorId = twit.Id
            };
            var key = "comment-add-" + DateTime.Now.ToString();
            var val = JObject.FromObject(comment).ToString(Formatting.None);
            string topic = "comment";
            await KafkaHelper.SendKafkaAsync(_kafkaSettings.Value, "logging", key, val);
            return await KafkaHelper.SendKafkaAsync(_kafkaSettings.Value, topic, key, val);
        }
    }
}