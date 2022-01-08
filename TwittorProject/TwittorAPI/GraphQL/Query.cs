using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate;
using Microsoft.AspNetCore.Http;
using TwittorAPI.Models;

namespace TwittorAPI.GraphQL
{
    public class Query
    {
        private readonly IHttpContextAccessor _httpContext;

        public Query(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor;
        }
        public IQueryable<User> GetUsers([Service] AppDbContext context) 
        {
            return context.Users;
        }

        public IQueryable<Comment> GetComments([Service] AppDbContext context)
        {
            return context.Comments;
        }
        
        public IQueryable<Twittor> GetTwittors([Service] AppDbContext context)
        {
            return context.Twittors;
        }

        public IQueryable<Role> GetRoles([Service] AppDbContext context)
        {
            return context.Roles;
        }

        public IQueryable<UserRole> GetUserRoles([Service] AppDbContext context)
        {
            return context.UserRoles;
        }

        public IQueryable<User> GetUserProfile([Service] AppDbContext context)
        {
            var userId = _httpContext.HttpContext.User.FindFirst("Id").Value;
            return context.Users.Where(user=>user.Id == Convert.ToInt32(userId));
        }
    }
}