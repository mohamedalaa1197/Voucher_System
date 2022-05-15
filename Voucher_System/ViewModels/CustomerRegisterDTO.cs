using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voucher_System.Models;

namespace Voucher_System.ViewModels
{
    public class CustomerRegisterDTO
    {
        public string Email { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public DateTime dateOfBirth { get; set; }
        public gender gender { get; set; }
    }
}
