using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Voucher_System.Models
{
    public class BaseModel
    {
        public Guid Id { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string userName { get; set; }
        public Byte[] passwordHash { get; set; }
        public Byte[] passwordSalt { get; set; }
        public DateTime dateOfBirth { get; set; }
        public DateTime Created { get; set; }
        public DateTime lastActive { get; set; }
        public gender Gender { get; set; }

    }

    public enum gender
    {
        Male,
        Female
    }
}
