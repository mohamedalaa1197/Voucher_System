using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Voucher_System.Models
{
    [Table("transaction")]
    public class Transaction
    {
        [Key]
        [DefaultValue(-1)]
        [Column("id")] public int Id { get; set; }
        [Column("user_id")] public int UserId { get; set; }
        [Column("bouquet_id")] public int? BouquetId { get; set; }
        [Column("transaction_type")] public CommunityTransactionType Type { get; set; }
        [Column("credit")] public double Credit { get; set; }
        [Column("debit")] public double Debit { get; set; }
        [Column("currency")] public string Currency { get; set; }
        [Column("exchange_rate")] public double ExchangeRate { get; set; }
        [Column("debit_usd")] public double DebitUsd { get; set; }
        [Column("credit_usd")] public double CreditUsd { get; set; }
        [Column("balance_usd")] public double BalanceUsd { get; set; }
        [Column("notes")] public string Note { get; set; }
        [Column("create_date")] public DateTime CreateDate { get; set; }
        [Column("created_by")] public int? CreatedBy { get; set; }
        [Column("status")] public TransactionStatus Status { get; set; }
        [Column("product_sell_price_usd")] public double ProductSellPriceUSD { get; set; }
        [Column("product_sell_price")] public double ProductSellPrice { get; set; }

        public virtual TransactionExtended TransactionExtended { get; set; }
    }

    public enum CommunityTransactionType
    {
        Credit = 0,
        Debit = 1,
        difference = 2
    }

    public enum TransactionStatus
    {
        Hold = 0,
        Confirmed = 1,
        Rejected = 2
    }
}
