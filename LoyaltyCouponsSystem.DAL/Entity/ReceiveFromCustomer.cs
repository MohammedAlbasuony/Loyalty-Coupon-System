using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.DAL.Entity
{
    public class ReceiveFromCustomer
    {
        public int Id { get; set; } // Primary Key
        public string CustomerCode { get; set; } // Changed to string
        public string DistributorCode { get; set; } // Changed to string
        public string TechnicianCode { get; set; } // Changed to string
        public string Governorate { get; set; } // Changed to string
        public string City { get; set; } // Changed to string
        public long CouponReceiptNumber { get; set; } // Numeric
        public DateTime TransactionDate { get; set; }
    }
}
