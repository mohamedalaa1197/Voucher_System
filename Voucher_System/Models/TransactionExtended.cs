using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Voucher_System.Models
{
    [Table("transaction_extended")]
    public class TransactionExtended
    {
        [Key]
        [Column("transaction_id")]
        public int TransactionId { get; set; }
        [Column("order_id")] public int OrderId { get; set; }
        [Column("prod_id")] public int ProductId { get; set; }
        [Column("cart_id")] public int CartId { get; set; }
        public virtual Transaction Transaction { get; set; }
    }
}
