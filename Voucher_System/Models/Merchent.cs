using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voucher_System.Models
{
    public class Merchent : BaseModel
    {
        public IEnumerable<Voucher> Vouchers { get; set; }
    }
}
