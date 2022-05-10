using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voucher_System.Helpers;

namespace Voucher_System.Models
{
    public class Voucher
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Points { get; set; }
        public string Image { get; set; }
        public VoucherType VoucherType { get; set; } = VoucherType.Discount;
        public int NoOfUsage { get; set; }
        public VoucherCategory VoucherCategory { get; set; } = VoucherCategory.Silver;
        public Merchent Merchent { get; set; }
        public Guid MerchentId { get; set; }
        public ICollection<CustomerVoucher> CustomerVouchers { get; set; }
    }
}
