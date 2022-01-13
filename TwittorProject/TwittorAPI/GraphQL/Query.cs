using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate;
using Microsoft.AspNetCore.Http;
using TwittorAPI.Models;
using HotChocolate.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using TwittorAPI.Kafka;
using Microsoft.Extensions.Options;

namespace TwittorAPI.GraphQL
{
    public class Query
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IOptions<KafkaSettings> _kafkaSettings;

        public Query([Service] IOptions<KafkaSettings> kafkaSettings, IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor;
            _kafkaSettings = kafkaSettings;
        }
        public async Task<IQueryable<User>> GetUsersAsync([Service] AppDbContext context) 
        {
            
            var user = context.Users;
            var key = "users_get-" + DateTime.Now.ToString();
            await KafkaHelper.SendKafkaAsync(_kafkaSettings.Value, "logging", key, "get all users");
            return user;
        }

        public async Task<IQueryable<Comment>> GetComments([Service] AppDbContext context)
        {
            var comments = context.Comments;      
            var key = "comments_get--" + DateTime.Now.ToString();
            await KafkaHelper.SendKafkaAsync(_kafkaSettings.Value, "logging", key, "get all comments");
            return comments;
        }
        
        public async Task<IQueryable<Twittor>> GetTwittorsAsync([Service] AppDbContext context)
        {
            var twittor = context.Twittors;
            var key = "twittors-get" + DateTime.Now.ToString();
            await KafkaHelper.SendKafkaAsync(_kafkaSettings.Value, "logging", key, "get all twittors");
            return twittor;
        }

        public IQueryable<Role> GetRoles([Service] AppDbContext context)
        {
            return context.Roles;
        }

        public IQueryable<UserRole> GetUserRoles([Service] AppDbContext context)
        {
            return context.UserRoles;
        }

        [Authorize(Roles = new [] {"user"})]
        public async Task<IQueryable<User>> GetUserProfileAsync([Service] AppDbContext context)
        {
            var userId = _httpContext.HttpContext.User.FindFirst("Id").Value;
            var user = context.Users.Where(user=>user.Id == Convert.ToInt32(userId));
            var key = "user-get-profile-" + DateTime.Now.ToString();
            var val = JObject.FromObject(user.SingleOrDefault()).ToString(Formatting.None);
            await KafkaHelper.SendKafkaAsync(_kafkaSettings.Value, "logging", key, val);
            return user;
        }
    }
}