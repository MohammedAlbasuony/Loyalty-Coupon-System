using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LoyaltyCouponsSystem.BLL.ViewModel.Technician
{
    public class TechnicianViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        [Required(ErrorMessage = "National ID is required.")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be exactly 14 digits long.")]
        [Display(Name = "National ID")]
        public string NationalID { get; set; }
        public int PhoneNumber1 { get; set; }
        public int? PhoneNumber2 { get; set; }
        public int? PhoneNumber3 { get; set; }
        public List<SelectListItem> Governates { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Cities { get; set; } = new List<SelectListItem>();
        public string SelectedGovernate { get; set; }
        public string SelectedCity { get; set; }
    }

}
