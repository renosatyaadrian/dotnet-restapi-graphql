using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwittorAPI.Constants
{
    public record UserToken
    (
        string Token,
        string Expired,
        string Message
    );
}