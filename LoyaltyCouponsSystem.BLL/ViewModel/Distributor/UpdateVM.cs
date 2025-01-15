using LoyaltyCouponsSystem.BLL.ViewModel.Customer;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.BLL.ViewModel.Distributor
{
    public class UpdateVM
    {
        public int DistributorID { get; set; }

        [Required(ErrorMessage = "Account Number is required.")]
        [StringLength(20, ErrorMessage = "Account Number must be less than 20 digits.")]
        public string Code { get; set; }

        public string Name { get; set; }
        public bool IsDeleted { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Phone number must contain only digits.")]
        [Display(Name = "Primary Phone Number")]
        public int PhoneNumber1 { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }

        public string? SelectedGovernate { get; set; }
        public string? SelectedCity { get; set; }
        public List<SelectListItem> Governates { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Cities { get; set; } = new List<SelectListItem>();
        public List<string>? SelectedCustomerCodes { get; set; }
        public List<string>? SelectedCustomerNames { get; set; }
        public List<CustomerViewModel>? AvailableCustomers { get; set; }
        public List<SelectListItem> Customers { get; set; } = new List<SelectListItem>();
    }
}
