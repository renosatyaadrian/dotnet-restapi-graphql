using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwittorAPI.GraphQL
{
    public record RegisterUserInput
    (
        int? Id,
        string Username,
        string Email,
        string FullName,
        string? Password
    );
}