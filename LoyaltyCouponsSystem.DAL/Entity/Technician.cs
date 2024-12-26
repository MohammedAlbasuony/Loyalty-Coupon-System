using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.DAL.Entity
{
    public class Technician
    {
        public int TechnicianID { get; set; }
        public string Name { get; set; } 
        public string ContactDetails { get; set; }
        public ICollection<Coupon> Coupons { get; set; }
    }
}
