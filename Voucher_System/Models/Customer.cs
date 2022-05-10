using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voucher_System.Models
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Points { get; set; }
        public ICollection<CustomerVoucher> CustomerVouchers { get; set; }
    }
}
