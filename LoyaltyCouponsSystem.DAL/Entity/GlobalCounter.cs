using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.DAL.Entity
{
    public class GlobalCounter
    {
        public long MaxSerialNumber {  get; set; }
        public long MaXNumberInYear { get; set; }
        [Key]
        public int Year { get; set; }

        public int YearNotId { get; set; }



    }
}
