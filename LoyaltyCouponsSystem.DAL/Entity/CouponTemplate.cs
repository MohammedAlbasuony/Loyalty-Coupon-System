using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.DAL.Entity
{
    public class CouponTemplate
    {
        public CouponTemplate(int Count, int CouponsPerRow, float CouponSize, float VerticalSpacing, float HorizontalSpacing)

        {
            this.Count = Count;
            this.CouponsPerRow = CouponsPerRow;
            this.CouponSize = CouponSize;
            this.VerticalSpacing = VerticalSpacing;
            this.HorizontalSpacing = HorizontalSpacing;

        }

        // Parameterless constructor required by EF
        public CouponTemplate() { }
       
        public int Count { get; } //Number Coupone Want to print it

        public float CouponSize { get; }
        public float CouponsPerRow { get; }

        public float VerticalSpacing { get; }

        public float HorizontalSpacing { get; }
       
       
       
        

        

        
    }
}
