using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwittorAPI.GraphQL
{
    public record RegisterUserInput
    (
        string Username,
        string Email,
        string FullName,
        string Password
    );
}