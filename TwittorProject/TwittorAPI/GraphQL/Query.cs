using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate;
using TwittorAPI.Models;

namespace TwittorAPI.GraphQL
{
    public class Query
    {
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
    }
}