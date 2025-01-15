using System.ComponentModel.DataAnnotations;

namespace LoyaltyCouponsSystem.BLL.ViewModel.Customer
{
    public class CustomerViewModel
    {
        [Required(ErrorMessage = "Name is required.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Name must contain only letters, numbers, and spaces.")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Account Number is required.")]
        [StringLength(20, ErrorMessage = "Account Number must be less than 20 digits.")]
        [UniqueCode(ErrorMessage = "This account number is already in use.")]
        public string Code { get; set; }

        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Governate must contain only letters and spaces.")]
        public string? Governate { get; set; }

        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "City must contain only letters and spaces.")]
        public string? City { get; set; }

        [Phone(ErrorMessage = "Please provide a valid phone number.")]
        [RegularExpression(@"^01[0-9]{9}$", ErrorMessage = "Phone number must follow the format 01xxxxxxxxx.")]
        [UniquePhoneNumber(ErrorMessage = "This phone number is already in use.")]
        public string? PhoneNumber { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
