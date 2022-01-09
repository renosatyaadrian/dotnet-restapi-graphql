using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwittorAPI.Input
{
    public record UpdatePasswordUserInput
    (
        int? Id,
        string oldPassword,
        string newPassword
    );
}