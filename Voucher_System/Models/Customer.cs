using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voucher_System.Models
{
    public class Customer : BaseModel
    {
        public int Points { get; set; }
        public ICollection<CustomerVoucher> CustomerVouchers { get; set; }
    }
}
