using LoyaltyCouponsSystem.DAL.DB;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class UniqueCodeAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var context = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext));
        string code = value?.ToString()?.Trim();  // Trim the code to remove leading/trailing spaces

        if (string.IsNullOrEmpty(code)) return ValidationResult.Success;

        // Use EF.Functions.Like() for case-insensitive comparison
        var existsCustomer = context.Customers
            .Any(c => EF.Functions.Like(c.Code, code));

        var existsDistributor = context.Distributors
            .Any(d => EF.Functions.Like(d.Code, code) && !d.IsDeleted);

        var existsTechnician = context.Technicians
            .Any(t => EF.Functions.Like(t.Code, code));

        if (existsCustomer || existsDistributor || existsTechnician)
        {
            return new ValidationResult("The account number (Code) is already in use.");
        }

        return ValidationResult.Success;
    }
}
