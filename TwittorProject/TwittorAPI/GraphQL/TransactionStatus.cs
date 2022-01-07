using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwittorAPI.GraphQL
{
    public record TransactionStatus
    (
        bool Succeed,
        string? Message
    );
}