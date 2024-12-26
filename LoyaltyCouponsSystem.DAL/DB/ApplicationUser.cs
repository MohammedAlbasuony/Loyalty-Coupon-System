using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.DAL.DB
{
    public class ApplicationUser : IdentityUser
    {
        public string Role_Type { get; set; }
        public string Phone { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string? Imagepath { get; set; }
    }
}
