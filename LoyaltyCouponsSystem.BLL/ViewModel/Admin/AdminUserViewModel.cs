using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.BLL.ViewModel.Admin
{
    public class AdminUserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public bool IsDeleted { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }       
    }
}
