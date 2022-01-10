using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnrollmentService.Dtos;
using EnrollmentService.Models;

namespace EnrollmentService.Data
{
    public interface IUser
    {
        Task AddRole(string rolename);
        Task AddUserToRole(string username, string role);
        Task<User> Authenticate(string username, string password);
        IEnumerable<UserDto> GetAllUser();
        IEnumerable<CreateRoleDto> GetAllRole();
        Task<List<string>> GetUserRoles(string username);
        Task Registration(CreateUserDto user);
    }
}