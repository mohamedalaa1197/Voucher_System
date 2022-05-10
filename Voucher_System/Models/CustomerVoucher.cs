using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voucher_System.Models
{
    public class CustomerVoucher
    {
        public Customer Customer { get; set; }
        public Guid CustomerId { get; set; }
        public Voucher Voucher { get; set; }
        public int VoucherId { get; set; }
    }
}
