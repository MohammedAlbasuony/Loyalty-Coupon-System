using Microsoft.AspNetCore.Mvc.Rendering;

namespace LoyaltyCouponsSystem.BLL.ViewModel.Customer_and_Technician
{

    public class AssignmentViewModel
    {
        public List<SelectListItem> Customers { get; set; }
        public List<SelectListItem> Technicians { get; set; }
        public List<SelectListItem> Governates { get; set; }
        public List<SelectListItem> Cities { get; set; }
        public List<SelectListItem> CouponSorts { get; set; }
        public List<SelectListItem> CouponTypes { get; set; }

        public string SelectedCustomerCode { get; set; }
        public string SelectedTechnicianCode { get; set; }
        public string SelectedCouponSort { get; set; }
        public string SelectedCouponType { get; set; }
        public string SelectedGovernate { get; set; }
        public string SelectedCity  {  get; set;  }
        public int StartSequenceNumber { get; set; }
        public int EndSequenceNumber { get; set; }
        public string ExchangePermission { get; set; }
        public string CustomerDetails { get; set; }
        public string TechnicianDetails { get; set; }
    }

}
