using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voucher_System.Models
{
    public class Merchent
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<Voucher> Vouchers { get; set; }
    }
}
