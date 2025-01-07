using LoyaltyCouponsSystem.DAL.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.BLL.ViewModel.QRCode
{
    public class QRCodeDetailsViewModel
    {
        public string TypeOfCoupon { get; set; }

        public int Value { get; set; }

        public int Count { get; set; }

        public long SerialNumber { get; set; }

        public float CouponSize{ get; set; }

        

        public float VerticalSpacing { get; set; }

        public float HorizontalSpacing { get; set; }

        public int CouponsPerRow { get; set; }


        [Display(Name = "Governorate")]
        public int GovernorateId { get; set; }



        public IEnumerable<Governorate>? governorates { get; set; }

        [Display(Name = "Area")]

        public int AreaId { get; set; }
        public IEnumerable<Area>? Areas { get; set; } = new List<Area>();


    }
}
