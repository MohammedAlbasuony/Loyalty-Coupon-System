using System.ComponentModel.DataAnnotations;

namespace LoyaltyCouponsSystem.BLL.ViewModel.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Name is required.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Name must contain only letters, numbers, and spaces.")]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "National ID is required.")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be exactly 14 digits long.")]
        [Display(Name = "National ID")]
        public string NationalID { get; set; }

        [Required(ErrorMessage = "Phone Number is required.")]
        [RegularExpression(@"^01[0-9]{9}$", ErrorMessage = "Phone number must follow the format 01xxxxxxxxx.")]
        public string PhoneNumber { get; set; }

        [RegularExpression(@"^01[0-9]{9}$", ErrorMessage = "Phone number must follow the format 01xxxxxxxxx.")]
        public string? OptionalPhoneNumber { get; set; }

        [Required(ErrorMessage = "Governorate is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Governorate must contain only letters and spaces.")]
        public string Governorate { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "City must contain only letters and spaces.")]
        public string City { get; set; }

        [Required]
        public string Role { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "The password must be at least 8 characters long.")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "Passwords do not match.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

    }
}
