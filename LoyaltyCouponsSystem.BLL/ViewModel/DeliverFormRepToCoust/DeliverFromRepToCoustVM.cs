using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.BLL.ViewModel.DeliverFormRepToCoust
{
    public class DeliverFromRepToCoustVM
    {
        public List<SelectListItem> Customers { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Technicians { get; set; } = new List<SelectListItem>();


        [Required(ErrorMessage = "Customer is required.")]
        public string? SelectedCustomerCode { get; set; }

        [Required(ErrorMessage = "Technician is required.")]
        public string? SelectedTechnicianCode { get; set; }

        public string? Governorate { get; set; }
        public string? Area { get; set; }
       


        [Required(ErrorMessage = "Exchange permission is required.")]
        [StringLength(50, ErrorMessage = "Exchange permission cannot exceed 50 characters.")]
        [UniqueExchangePermission(ErrorMessage = "Exchange permission must be unique.")]
        public string ExchangePermission { get; set; }
        public string CustomerDetails { get; set; }
        public string TechnicianDetails { get; set; }
        public string? CreatedBy { get; set; }
        
    }
}
