using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.DAL.Entity
{
    public class Coupon
    {


        
        public string CouponeId { get; private set; } = Guid.NewGuid().ToString();
        public long SerialNumber { get; set; }

        public string TypeOfCoupone { get; set; }
        public string Status { get; set; } = "Created";
        public DateTime CreationDateTime { get; set; } = DateTime.Now;
       
      
        public DateTime? ClosureDate { get; set; }
        public ICollection<Representative> Representatives { get; set; } 
        public int? RepresentativeId { get; set; }
        public ICollection<Technician> Technicians { get; set; }
        public int? TechnicianId { get; set; }
        public ICollection<StoreKeeper> StoreKeepers { get; set; }
        
        public int ?StorekeeperID { get; set; }

        public int? EmployeWhoGenerateID { get; set; }

        public int Value { get; set; }

        public int? GovernorateId { get; set; }

        public int? AreaId { get; set; }
        public long NumInYear { get; set; }

       

    }
}
