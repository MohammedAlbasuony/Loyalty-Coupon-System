using LoyaltyCouponsSystem.BLL.ViewModel.Customer;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace LoyaltyCouponsSystem.BLL.ViewModel.Distributor
{
    public class DistributorViewModel
    {
        public int DistributorID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public int PhoneNumber1 { get; set; }
        public string? SelectedGovernate { get; set; }
        public string? SelectedCity { get; set; }
        public List<SelectListItem> Governates { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Cities { get; set; } = new List<SelectListItem>();
        public string SelectedCustomerCode { get; set; }
        public List<SelectListItem> Customers { get; set; } = new List<SelectListItem>();

    }
}
