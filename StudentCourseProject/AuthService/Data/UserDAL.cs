using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthService.Dtos;
using AuthService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Data
{
    public class UserDAL : IUser
    {
       private readonly AppSettings _appSettings;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public UserDAL(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<AppSettings> appSetting)
        {
            _appSettings = appSetting.Value;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task AddRole(string rolename)
        {
            try
            {
                 var roleIsExist = await _roleManager.RoleExistsAsync(rolename);
                 if(roleIsExist) 
                    throw new Exception($"Role {rolename} sudah terdaftar");
                 else 
                    await _roleManager.CreateAsync(new IdentityRole(rolename));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task AddUserToRole(string username, string role)
        {
            var result = await _userManager.FindByNameAsync(username);
            try
            {
                await _userManager.AddToRoleAsync(result, role);    
            }
            catch (Exception ex)
            {   
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> Authenticate(string username, string password)
        {
            var userFind = await _userManager.CheckPasswordAsync(await _userManager.FindByNameAsync(username), password);
            if(!userFind)
                return null;
            var user = new User
            {
                Username = username
            };

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.Username));
            var roles = await GetUserRoles(username);

            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            return user;
        }

        public IEnumerable<CreateRoleDto> GetAllRole()
        {
            List<CreateRoleDto> roles = new List<CreateRoleDto>();
            var results =  _roleManager.Roles;
            foreach(var role in results)
            {
                roles.Add(new CreateRoleDto{ RoleName = role.Name });
            }
            return roles;
        }

        public IEnumerable<UserDto> GetAllUser()
        {
            List<UserDto> users = new List<UserDto>();
            var results = _userManager.Users;
            foreach(var user in results)
            {
                users.Add(new UserDto{ Username = user.UserName});
            }
            return users;
        }

        public async Task<List<string>> GetUserRoles(string username)
        {
            List<string> lstRoles = new List<string>();
            var result = await _userManager.FindByEmailAsync(username);
            if(result==null)
                throw new Exception($"User {username} tidak ditemukan");
            var roles =  await _userManager.GetRolesAsync(result);
            foreach(var role in roles)
            {
                lstRoles.Add(role);
            }
            return lstRoles;
        }

        public async Task Registration(CreateUserDto user)
        {
            try
            {
                var newUser = new IdentityUser
                {
                    UserName = user.Username, 
                    Email = user.Username
                };
                var result = await _userManager.CreateAsync(newUser, user.Password);
                if(!result.Succeeded)
                    throw new Exception($"Gagal menambahkan user {user.Username}. Error: {result.Errors.Select(error=>error.Description)}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        } 
    }
}