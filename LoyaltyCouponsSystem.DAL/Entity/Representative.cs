using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.DAL.Entity
{
    public class Representative
    {
        public int RepresentativeID { get; set; }
        public string Name { get; set; }
        public string Number { get; set; } 
        public string ApprovalStatus { get; set; } 
        public ICollection<Coupon> Coupons { get; set; }
    }
}
