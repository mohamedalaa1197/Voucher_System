using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voucher_System.Models;

namespace Voucher_System.Services.Interfaces
{
    public interface ITokenService
    {
        string creatToken(string userName);
    }
}
