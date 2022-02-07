using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TwittorAPI.Input;
using TwittorAPI.Kafka;
using TwittorAPI.Models;
using TwittorAPI.Constants;
using HotChocolate.Types;

namespace TwittorAPI.GraphQL.Mutations
{
    [ExtendObjectType(Name = "Mutation")]
    [Obsolete]
    public class AdminMutation
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<KafkaSettings> _kafkaSettings;

        public AdminMutation([Service] IOptions<KafkaSettings> kafkaSettings, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _kafkaSettings = kafkaSettings;
        }

        [Authorize(Roles = new [] {"admin"})]
        public async Task<TransactionStatus> LockUserAsync([Service] AppDbContext context, [Service] IOptions<KafkaSettings> kafkaSettings, LockUserInput input)
        {
            var user = context.Users.Where(user=>user.Id == input.UserId).FirstOrDefault();
            if(user == null)
            {
                return await Task.FromResult(new TransactionStatus(false, "User not found"));
            }
            user.IsLocked = input.IsLocked;
            var key = "user-lock-" + DateTime.Now.ToString();
            var val = JObject.FromObject(user).ToString(Formatting.None);
            string topic = "user-update";
            await KafkaHelper.SendKafkaAsync(kafkaSettings.Value, "logging", key, val);
            return await KafkaHelper.SendKafkaAsync(kafkaSettings.Value, topic, key, val);
        }
        
        public async Task<TransactionStatus> CreateRoleAsync([Service] AppDbContext context, [Service] IOptions<KafkaSettings> kafkaSettings, CreateRoleInput input)
        {
            var role = context.Roles.Where(role=>role.RoleName.ToLower()==input.RoleName.ToLower()).SingleOrDefault();
            if(role!=null)
            {
                return await Task.FromResult(new TransactionStatus(false, "Role already exist"));
            }
            var newRole = new Role
            {
                RoleName = input.RoleName
            };

            var key = "role-add-" + DateTime.Now.ToString();
            var val = JObject.FromObject(newRole).ToString(Formatting.None);
            string topic = "role-add";
            await KafkaHelper.SendKafkaAsync(kafkaSettings.Value, "logging", key, val);
            return await KafkaHelper.SendKafkaAsync(kafkaSettings.Value, topic, key, val);
        }

        public async Task<TransactionStatus> CreatOrUpdateUserRoleAsync([Service] AppDbContext context,[Service] IOptions<KafkaSettings> kafkaSettings, CreateOrUpdateUserRoleInput input)
        {
            var user = context.Users.Where(user=>user.Username.ToLower()==input.Username.ToString().ToLower()).SingleOrDefault();
            if(user==null)
            {
                return await Task.FromResult(new TransactionStatus(false, "User not found"));
            }
            var role = context.Roles.Where(role=>role.RoleName.ToLower()==input.RoleName.ToString().ToLower()).SingleOrDefault();
            if(role==null)
            {
                return await Task.FromResult(new TransactionStatus(false, "Role not found"));
            }
            var userRole = context.UserRoles.Where(userRole=>userRole.UserId==user.Id && userRole.RoleId==role.Id).SingleOrDefault();
            if(userRole!=null)
            {
                return await Task.FromResult(new TransactionStatus(false, "UserRole already exist"));
            }
            var newUserRole = new UserRole
            {
                RoleId = role.Id,
                UserId = user.Id
            };

            var key = "user-role-add-" + DateTime.Now.ToString();
            var val = JObject.FromObject(newUserRole).ToString(Formatting.None);
            string topic = "user-role-add";
            await KafkaHelper.SendKafkaAsync(kafkaSettings.Value, "logging", key, val);
            return await KafkaHelper.SendKafkaAsync(kafkaSettings.Value, topic, key, val);
        }
    }
}