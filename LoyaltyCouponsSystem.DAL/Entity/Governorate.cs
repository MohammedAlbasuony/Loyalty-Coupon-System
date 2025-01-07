using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.DAL.Entity
{
    public class Governorate
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Area> Areas { get; set; } // العلاقة One-to-Many
    }
}
