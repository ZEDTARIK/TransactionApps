using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransactionsApps.Models
{
    /*
     Code SQL : 

    CREATE TABLE [Transactions] (
      [TransactionId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
      [AccountNumber] NVARCHAR(50) NOT NULL,
      [BankName]      NVARCHAR(20) NOT NULL,
      [SWIFTCode]     NVARCHAR(5)  NOT NULL,
      [Amount]        DECIMAL(18,2) NOT NULL,
      [DateCreation]  DATETIME2 NOT NULL
    );
     */

    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "This field is required!")]
        [MaxLength(50)]
        [Display(Name = "Account Number")] // For UI Razor 
        public string AccountNumber { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        [Required(ErrorMessage = "This field is required!")]
        [Display(Name = "Bank Name")]
        [MaxLength(20)]
        public string BankName { get; set; }

        [Column(TypeName = "nvarchar(5)")]
        [Required(ErrorMessage = "This field is required!")]
        [Display(Name = "SWIFT CODE")]
        [MaxLength(5)]
        public string SWIFTCode { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Required(ErrorMessage = "This field is required!")]
        public decimal Amount { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Date")]
        public DateTime DateCreation { get; set; }
    }
}
