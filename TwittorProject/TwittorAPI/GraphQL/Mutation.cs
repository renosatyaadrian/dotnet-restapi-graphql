using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using HotChocolate;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TwittorAPI.Models;

namespace TwittorAPI.GraphQL
{
    public class Mutation
    {
        public async Task<UserData> RegisterUserAsync([Service] AppDbContext context, RegisterUserInput input)
        {
            var user = context.Users.Where(user=>user.Username==input.Username).FirstOrDefault();
            if(user != null)
            {
                return await Task.FromResult(new UserData());
            }
            
            var newUser = new User
            {
                FullName = input.FullName,
                Email = input.Email.ToLower(),
                Username = input.Username.ToLower(),
                Password = BCrypt.Net.BCrypt.HashPassword(input.Password)
            };
            
            var res = context.Users.Add(newUser);
            await context.SaveChangesAsync();

            return await Task.FromResult(new UserData
                {
                    Id = newUser.Id,
                    FullName = input.FullName,
                    Email = input.Email,
                    Username = input.Username,
                    Created = DateTime.Now
                });
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

        public async Task<Role> CreateRoleAsync([Service] AppDbContext context, CreateRoleInput input)
        {
            var role = context.Roles.Where(role=>role.RoleName.ToLower()==input.RoleName.ToLower()).SingleOrDefault();
            if(role!=null)
            {
                return await Task.FromResult(new Role());
            }
            var newRole = new Role
            {
                RoleName = input.RoleName
            };
            var res = context.Roles.Add(newRole);
            await context.SaveChangesAsync();

            return await Task.FromResult(new Role{
                RoleName = newRole.RoleName
            });
        }

        public async Task<UserRole> CreatOrUpdateUserRoleAsync([Service] AppDbContext context, CreateOrUpdateUserRoleInput input)
        {
            var user = context.Users.Where(user=>user.Username.ToLower()==input.Username.ToString().ToLower()).SingleOrDefault();
            if(user==null)
            {
                return await ReturnNull();
            }
            var role = context.Roles.Where(role=>role.RoleName.ToLower()==input.RoleName.ToString().ToLower()).SingleOrDefault();
            if(role==null)
            {
                return await ReturnNull();
            }
            var userRole = context.UserRoles.Where(userRole=>userRole.UserId==user.Id && userRole.RoleId==role.Id).SingleOrDefault();
            if(userRole!=null)
            {
                return await ReturnNull();
            }
            var newUserRole = new UserRole
            {
                RoleId = role.Id,
                UserId = user.Id
            };

            var res = context.UserRoles.Add(newUserRole);
            await context.SaveChangesAsync();

            return await Task.FromResult(new UserRole
            {
                Id = newUserRole.Id,
                RoleId = role.Id,
                UserId = user.Id
            });
        }

        private async Task<UserRole> ReturnNull()
        {
            return await Task.FromResult(new UserRole());
        }
    }
}