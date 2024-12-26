using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.DAL.Entity
{
    public class CouponTemplate
    {
        public int TemplateID { get; set; }
        public string Name { get; set; } 
        public string Branding { get; set; } 
        public string DesignDetails { get; set; }
        public ICollection<Coupon> Coupons { get; set; } 
    }
}
