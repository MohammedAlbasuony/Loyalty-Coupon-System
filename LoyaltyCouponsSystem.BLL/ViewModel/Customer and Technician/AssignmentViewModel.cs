using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

public class AssignmentViewModel
{
    public List<SelectListItem> Customers { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> Technicians { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> Governates { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> Cities { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> CouponSorts { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> CouponTypes { get; set; } = new List<SelectListItem>();

    [Required(ErrorMessage = "Customer is required.")]
    public string SelectedCustomerCode { get; set; }

    [Required(ErrorMessage = "Technician is required.")]
    public string SelectedTechnicianCode { get; set; }

    [Required(ErrorMessage = "Coupon sort is required.")]
    public string SelectedCouponSort { get; set; }

    [Required(ErrorMessage = "Coupon type is required.")]
    public string SelectedCouponType { get; set; }

    [Required(ErrorMessage = "Governate is required.")]
    public string SelectedGovernate { get; set; }

    [Required(ErrorMessage = "City is required.")]
    public string SelectedCity { get; set; }

    [Required(ErrorMessage = "Start sequence number is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Start sequence number must be a positive number.")]
    public int StartSequenceNumber { get; set; }

    [Required(ErrorMessage = "End sequence number is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "End sequence number must be a positive number.")]
    public int EndSequenceNumber { get; set; }

    [Required(ErrorMessage = "Exchange permission is required.")]
    [StringLength(50, ErrorMessage = "Exchange permission cannot exceed 50 characters.")]
    public string ExchangePermission { get; set; }
    public string CustomerDetails { get; set; }
    public string TechnicianDetails { get; set; }
    public string? CreatedBy { get; set; }

}
