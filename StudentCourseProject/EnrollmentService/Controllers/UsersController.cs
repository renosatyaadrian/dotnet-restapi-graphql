using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnrollmentService.Data;
using EnrollmentService.Dtos;
using EnrollmentService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUser _user;

        public UsersController(IUser user)
        {
            _user = user;
        }

        [HttpPost]
        public async Task<ActionResult> Registration(CreateUserDto user)
        {
            try
            {
                await _user.Registration(user);
                return Ok($"Register user {user.Username} berhasil");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("UserInRole")]
        public async Task<ActionResult> AddUsersRole(string username, string role)
        {
            try
            {
                await _user.AddUserToRole(username, role);
                return Ok($"Update role user {username} berhasil");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserDto>> GetAllUser()
        {
            return Ok(_user.GetAllUser());
        }

        [HttpGet("RolesByUser/{username}")]
        public async Task<ActionResult<List<string>>> GetRolesByUser(string username)
        {
            try
            {
                var results = await _user.GetUserRoles(username);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Roles")]
        public ActionResult<IEnumerable<CreateRoleDto>> GetAllRole()
        {
            return Ok(_user.GetAllRole());
        }

        [HttpPost("Roles")]
        public async Task<ActionResult> AddRole([FromBody] CreateRoleDto rolename)
        {
            try
            {
                await _user.AddRole(rolename.RoleName);
                return Ok($"Role {rolename.RoleName} berhasil ditambahkan");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Authentication")]
        public async Task<ActionResult<User>> Authentication([FromBody] AuthenticateDto auth)
        {
            try
            {
                
                var user = await _user.Authenticate(auth.Username, auth.Password);
                if(user == null)
                    return BadRequest("Invalid username / password");
                return Ok(user);    
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}