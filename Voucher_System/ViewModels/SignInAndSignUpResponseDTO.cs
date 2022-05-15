using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Voucher_System.ViewModels
{
    public class SignInAndSignUpResponseDTO
    {
        [Required]
        [StringLength(8, MinimumLength = 4)]
        public string userName { get; set; }
        public string token { get; set; }
    }
}
