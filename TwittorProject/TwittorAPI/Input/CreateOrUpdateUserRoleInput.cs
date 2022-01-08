using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwittorAPI.GraphQL
{
    public record CreateOrUpdateUserRoleInput
    (
        string Username,
        string RoleName
    );
}