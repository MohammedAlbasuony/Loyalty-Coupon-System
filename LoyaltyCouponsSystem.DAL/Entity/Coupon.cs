using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.DAL.Entity
{
    public class Coupon
    {
        public int CouponID { get; set; }
        public string UniqueIdentifier { get; set; } 
        public string Status { get; set; } 
        public DateTime CreationDate { get; set; }
        public DateTime? ClosureDate { get; set; }
        public ICollection<Representative> Representatives { get; set; } 
        public ICollection<Technician> Technicians { get; set; } 
        public ICollection<StoreKeeper> StoreKeepers { get; set; }
    }
}
