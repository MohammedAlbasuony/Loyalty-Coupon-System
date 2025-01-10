using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.DAL.Entity
{
    public class Transaction
    {
        public int TransactionID { get; set; }
        public decimal PurchaseAmount { get; set; }
        public string? TransactionType { get; set; }
        public DateTime Timestamp { get; set; }

        public string? Governate { get; set; }
        public string? City { get; set; }
        public int CustomerID { get; set; }
        public Customer Customer { get; set; }
        public int TechnicianID { get; set; } 
        public Technician Technician { get; set; }
        public string? CouponSort { get; set; } 
        public string? CouponType { get; set; } 
        public int SequenceNumber { get; set; }
        public string ExchangePermission { get; set; }
        public string CreatedBy { get; set; }
    }
}
