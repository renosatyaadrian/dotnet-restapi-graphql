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

namespace TwittorAPI.GraphQL
{
    public class Mutation
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<KafkaSettings> _kafkaSettings;

        public Mutation([Service] IOptions<KafkaSettings> kafkaSettings, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _kafkaSettings = kafkaSettings;
        }
        public async Task<TransactionStatus> RegisterUserAsync([Service] AppDbContext context, [Service] IOptions<KafkaSettings> kafkaSettings, RegisterUserInput input)
        {
            var user = context.Users.Where(user=>user.Username==input.Username).FirstOrDefault();
            if(user != null)
            {
                return await Task.FromResult(new TransactionStatus(false, "User already registered"));
            }
            
            var newUser = new User
            {
                FullName = input.FullName,
                Email = input.Email.ToLower(),
                Username = input.Username.ToLower(),
                Password = BCrypt.Net.BCrypt.HashPassword(input.Password),
                IsLocked = false
            };
            
            var key = "user-register-" + DateTime.Now.ToString();
            var val = JObject.FromObject(newUser).ToString(Formatting.None);
            string topic = "user-add";
            await KafkaHelper.SendKafkaAsync(kafkaSettings.Value, "logging", key, val);
            return await KafkaHelper.SendKafkaAsync(kafkaSettings.Value, topic, key, val);
        }

        public async Task<UserToken> LoginUserAsync([Service] AppDbContext context, [Service] IOptions<TokenSettings> tokenSettings,LoginUserInput input)
        {
            var user = context.Users.Where(user=>user.Username.ToLower()==input.Username.ToLower()).SingleOrDefault();
            if(user == null)
            {
                return await Task.FromResult(new UserToken(null,null,"Invalid username or password"));
            }
            bool passwordValid = BCrypt.Net.BCrypt.Verify(input.Password, user.Password);
            if(passwordValid)
            {
                var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Value.Key));
                var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, user.Username));
                claims.Add(new Claim("Id", user.Id.ToString()));
                var userRoles = context.UserRoles.Where(userRole=>userRole.UserId==user.Id).ToList();
                foreach (var userRole in userRoles)
                {
                    var role = context.Roles.Where(o=>o.Id == userRole.RoleId).FirstOrDefault();
                    if(role!=null)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
                    }
                }
                var expired = DateTime.Now.AddHours(1);
                var jwtToken = new JwtSecurityToken(
                    issuer: tokenSettings.Value.Issuer,
                    audience: tokenSettings.Value.Audience,
                    expires: expired,   
                    claims: claims,
                    signingCredentials: credentials
                );

                var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
                return await Task.FromResult(new UserToken(token, expired.ToString(), null));
            }
            else return await Task.FromResult(new UserToken(null,null,"Invalid username or password"));
        }

        [Authorize(Roles = new [] { "user" })]
        public async Task<TransactionStatus> UpdateUserProfileAsync([Service] AppDbContext context, [Service] IOptions<KafkaSettings> kafkaSettings, RegisterUserInput input)
        {
            var user = new User();
            
            if(input.Id==null) 
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst("Id").Value;
                user = context.Users.Where(user=>user.Id==Convert.ToInt32(userId)).SingleOrDefault();
            }
            else user = context.Users.Where(user=>user.Id==input.Id).SingleOrDefault();
            user.Email = input.Email;
            user.FullName = input.FullName;
            user.Username = input.Username;

            var key = "user-update-" + DateTime.Now.ToString();
            var val = JObject.FromObject(user).ToString(Formatting.None);
            string topic = "user-update";
            await KafkaHelper.SendKafkaAsync(kafkaSettings.Value, "logging", key, val);
            return await KafkaHelper.SendKafkaAsync(kafkaSettings.Value, topic, key, val);
        }

        [Authorize(Roles = new [] {"admin", "user"})]
        public async Task<TransactionStatus> UpdatePasswordUserAsync([Service] AppDbContext context, [Service] IOptions<KafkaSettings> kafkaSettings, UpdatePasswordUserInput input)
        {
            var user = new User();
            if(input.Id==null) 
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst("Id").Value;
                user = context.Users.Where(user=>user.Id==Convert.ToInt32(userId)).SingleOrDefault();
            }
            else user = context.Users.Where(user=>user.Id==input.Id).SingleOrDefault();

            var valid = BCrypt.Net.BCrypt.Verify(input.oldPassword, user.Password);
            if(valid){
                user.Password = BCrypt.Net.BCrypt.HashPassword(input.newPassword);
            }
            else return new TransactionStatus(false, "Invalid password");
            var key = "user-update-password-" + DateTime.Now.ToString();
            var val = JObject.FromObject(user).ToString(Formatting.None);
            string topic = "user-update";
            await KafkaHelper.SendKafkaAsync(kafkaSettings.Value, "logging", key, val);
            return await KafkaHelper.SendKafkaAsync(kafkaSettings.Value, topic, key, val);
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
        
        [Authorize(Roles = new [] { "admin" })]
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
            var res = context.Roles.Add(newRole);
            await context.SaveChangesAsync();

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

        [Authorize(Roles = new [] { "user" })]
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
            string topic = "twittor-add";
            await KafkaHelper.SendKafkaAsync(_kafkaSettings.Value, "logging", key, val);
            return await KafkaHelper.SendKafkaAsync(_kafkaSettings.Value, topic, key, val);
        }

        [Authorize(Roles = new [] { "user" })]
        public async Task<TransactionStatus> DeleteTwittorAsync([Service] AppDbContext context, DeleteTwitInput input)
        {
            var twit = context.Twittors.Where(twit=>twit.Id==input.Id).SingleOrDefault();
            if(twit == null) return await Task.FromResult(new TransactionStatus(false, "Twit not found"));

            var key = "twittor-delete-" + DateTime.Now.ToString();
            var val = JObject.FromObject(twit).ToString(Formatting.None);
            string topic = "twittor-delete";
            await KafkaHelper.SendKafkaAsync(_kafkaSettings.Value, "logging", key, val);
            return await KafkaHelper.SendKafkaAsync(_kafkaSettings.Value, topic, key, val);
        }


        [Authorize(Roles = new [] { "user" })]
        public async Task<TransactionStatus> CreateTwitCommentAsync([Service] AppDbContext context, CommentTwitInput input)
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
            string topic = "comment-add";
            await KafkaHelper.SendKafkaAsync(_kafkaSettings.Value, "logging", key, val);
            return await KafkaHelper.SendKafkaAsync(_kafkaSettings.Value, topic, key, val);
        }
    }
}