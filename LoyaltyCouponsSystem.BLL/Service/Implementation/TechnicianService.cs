using DocumentFormat.OpenXml.Spreadsheet;
using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.BLL.ViewModel.Distributor;
using LoyaltyCouponsSystem.BLL.ViewModel.Technician;
using LoyaltyCouponsSystem.DAL.DB;
using LoyaltyCouponsSystem.DAL.Entity;
using LoyaltyCouponsSystem.DAL.Repo.Abstraction;
using LoyaltyCouponsSystem.DAL.Repo.Implementation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace LoyaltyCouponsSystem.BLL.Service.Implementation
{
    public class TechnicianService : ITechnicianService
    {
        private readonly ITechnicianRepo _technicianRepo;
        private readonly ICustomerRepo _customerRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        public TechnicianService(ITechnicianRepo technicianRepo, ICustomerRepo customerRepo, UserManager<ApplicationUser> userManager)
        {
            _technicianRepo = technicianRepo;
            _customerRepo = customerRepo;
            _userManager = userManager;
        }

        public async Task<bool> AddAsync(TechnicianViewModel technicianViewModel)
        {
            if (technicianViewModel != null)
            {
                var technician = new Technician
                {
                    Code = technicianViewModel.Code,
                    Name = technicianViewModel.Name,
                    NickName = technicianViewModel.NickName,
                    NationalID = technicianViewModel.NationalID,
                    PhoneNumber1 = technicianViewModel.PhoneNumber1,
                    PhoneNumber2 = technicianViewModel.PhoneNumber2,
                    PhoneNumber3 = technicianViewModel.PhoneNumber3,
                    Governate = technicianViewModel.SelectedGovernate,
                    City = technicianViewModel.SelectedCity,
                    CreatedAt = technicianViewModel?.CreatedAt,
                    CreatedBy = technicianViewModel?.CreatedBy,
                    IsActive = technicianViewModel.IsActive,
                };

                // Fetch customer IDs by selected codes
                var customerIds = await _customerRepo.GetCustomerIdsByCodesAsync(technicianViewModel.SelectedCustomerCodes);
                var customers = await _customerRepo.GetCustomersByIdsAsync(customerIds);

                if (customerIds == null || !customerIds.Any())
                {
                    // Handle the case when no customer IDs are found
                    return false;
                }

                if (customers == null || !customers.Any())
                {
                    // Handle the case when no customers are found
                    return false;
                }

                // Manually add each customer to the technician's Customers collection
                foreach (var customer in customers)
                {
                    if (customer != null)
                    {
                        technician.Customers.Add(customer);
                    }
                }

                var allUsers = await _userManager.Users.Where(u => technicianViewModel.SelectedUserCodes.Contains(u.Id))
                    .ToListAsync();

                // Filter the users asynchronously based on their role
                var users = new List<ApplicationUser>();
                foreach (var user in allUsers)
                {
                    if (await _userManager.IsInRoleAsync(user, "Representative"))
                    {
                        users.Add(user);
                    }
                }
                

                if (users == null || !users.Any())
                {
                    // Handle the case when no users are found
                    return false;
                }

                // Manually add each user to the technician's Users collection
                foreach (var user in users)
                {
                    if (user != null) // Ensure the user is not null before adding
                    {
                        technician.Users.Add(user);
                    }
                }

                // Save technician to the database
                return await _technicianRepo.AddAsync(technician);
            }

            return false;
        }




        public async Task<bool> DeleteAsync(int id)
        {
            if (id != 0)
            {
                return await _technicianRepo.DeleteAsync(id);
            }
            return false;
        }

        public async Task<List<TechnicianViewModel>> GetAllAsync()
        {
            var technicians = await _technicianRepo.GetAllAsync();

            // Fetch all Users and Customers
            var users = await _userManager.Users.ToListAsync();
            var customers = await _customerRepo.GetAllAsync(); // Assuming you have a method to get all customers

            var technicianViewModels = technicians.Select(technician => new TechnicianViewModel
            {
                TechnicianID = technician.TechnicianID,
                Code = technician.Code,
                Name = technician.Name,
                NickName = technician.NickName,
                NationalID = technician.NationalID,
                PhoneNumber1 = technician.PhoneNumber1,
                PhoneNumber2 = technician.PhoneNumber2,
                PhoneNumber3 = technician.PhoneNumber3,
                SelectedGovernate = technician.Governate,
                SelectedCity = technician.City,
                CreatedAt = technician.CreatedAt,
                CreatedBy = technician.CreatedBy,
                UpdatedBy = technician.UpdatedBy,
                UpdatedAt = technician.UpdatedAt,
                IsActive = technician.IsActive,
                SelectedCustomerNames = technician.Customers?.Select(c => c.Name).ToList(),
                SelectedUserNames = technician.Users?.Select(u => u.UserName).ToList(),
            }).ToList();

            // Iterate through each Technician to assign the User and Customer names
            foreach (var technicianViewModel in technicianViewModels)
            {
                // Assuming each technician has a list of associated users
                var associatedUsers = users.Where(u => technicianViewModel.TechnicianID == u.TechnicianId).ToList();
                foreach (var user in associatedUsers)
                {
                    technicianViewModel.SelectedUserNames.Add(user.UserName); // Add UserName to the list
                }

                // Assuming each technician has associated customers via their codes
                var associatedCustomers = customers.Where(c => technicianViewModel.TechnicianID == c.TechnicianId).ToList();
                foreach (var customer in associatedCustomers)
                {
                    technicianViewModel.SelectedCustomerNames.Add(customer.Name); // Add Customer Name to the list
                }

                PopulateDropdowns(technicianViewModel);
            }

            return technicianViewModels;
        }


        public async Task<UpdateTechnicianViewModel> GetByIdAsync(int id)
        {
            if (id != 0)
            {
                var technician = await _technicianRepo.GetByIdAsync(id);

                if (technician != null)
                {
                    var updateTechnicianViewModel = new UpdateTechnicianViewModel
                    {
                        TechnicianID = technician.TechnicianID,
                        Code = technician.Code,
                        Name = technician.Name,
                        NickName = technician.NickName,
                        NationalID = technician.NationalID,
                        PhoneNumber1 = technician.PhoneNumber1,
                        PhoneNumber2 = technician.PhoneNumber2,
                        PhoneNumber3 = technician.PhoneNumber3,
                        SelectedGovernate = technician.Governate,
                        SelectedCity = technician.City,
                        IsActive = technician.IsActive,
                        Customers = await GetCustomersForDropdownAsync(),
                        Users = await GetUsersForDropdownAsync(),
                    };

                    return updateTechnicianViewModel;
                }
            }
            return null;
        }

        public async Task<bool> UpdateAsync(UpdateTechnicianViewModel technicianViewModel)
        {
            if (technicianViewModel != null)
            {
                // Fetch the existing technician entity from the database
                var existingTechnician = await _technicianRepo.GetByIdAsync(technicianViewModel.TechnicianID);
                if (existingTechnician == null)
                {
                    return false; // Technician not found
                }

                // Update the fields
                existingTechnician.TechnicianID = technicianViewModel.TechnicianID;
                existingTechnician.Code = technicianViewModel.Code;
                existingTechnician.Name = technicianViewModel.Name;
                existingTechnician.NickName = technicianViewModel.NickName;
                existingTechnician.NationalID = technicianViewModel.NationalID;
                existingTechnician.PhoneNumber1 = technicianViewModel.PhoneNumber1;
                existingTechnician.PhoneNumber2 = technicianViewModel.PhoneNumber2;
                existingTechnician.PhoneNumber3 = technicianViewModel.PhoneNumber3;
                existingTechnician.Governate = technicianViewModel.SelectedGovernate;
                existingTechnician.City = technicianViewModel.SelectedCity;
                existingTechnician.UpdatedAt = DateTime.Now;
                existingTechnician.UpdatedBy = technicianViewModel.UpdatedBy;
                existingTechnician.IsActive = technicianViewModel.IsActive;
                // Save the updated entity
                return await _technicianRepo.UpdateAsync(existingTechnician);
            }
            return false;
        }


        public async Task<List<SelectListItem>> GetCustomersForDropdownAsync()
        {
            var customers = await _technicianRepo.GetCustomersForDropdownAsync();

            return customers.Select(c => new SelectListItem
            {
                Value = c.Code,
                Text = $"{c.Code} - {c.Name}"
            }).ToList();
        }
        public async Task<List<SelectListItem>> GetUsersForDropdownAsync()
        {
            // Fetch all active users from the database
            var activeUsers = await _userManager.Users
                                                .Where(user => user.IsActive) 
                                                .ToListAsync();

            // Filter the active users by role asynchronously
            var representatives = new List<ApplicationUser>();
            foreach (var user in activeUsers)
            {
                if (await _userManager.IsInRoleAsync(user, "Representative"))
                {
                    representatives.Add(user);
                }
            }

            // Return the list of SelectListItem
            return representatives.Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = $"{u.UserName}" // Display username
            }).ToList();
        }

        private void PopulateDropdowns(TechnicianViewModel viewModel)
        {
            // Hardcoded data for governates and cities
            var governates = new List<SelectListItem>
            {
                new SelectListItem { Text = "Cairo", Value = "Cairo" },
                new SelectListItem { Text = "Giza", Value = "Giza" },
                new SelectListItem { Text = "Alexandria", Value = "Alexandria" }
            };

            var cities = new List<SelectListItem>
            {
                new SelectListItem { Text = "Nasr City", Value = "Nasr City" },
                new SelectListItem { Text = "Dokki", Value = "Dokki" },
                new SelectListItem { Text = "Smouha", Value = "Smouha" }
            };

            viewModel.Governates = governates;
            viewModel.Cities = cities;
        }
        public async Task<bool> ToggleActivationAsync(int technicianId)
        {
            var technician = await _technicianRepo.GetByIdAsync(technicianId);
            if (technician == null)
            {
                return false;
            }

            technician.IsActive = !technician.IsActive;
            technician.UpdatedAt = DateTime.Now;

            return await _technicianRepo.UpdateAsync(technician);
        }

        public async Task<bool> ImportTechniciansFromExcelAsync(Stream stream)
        {
            try
            {
                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                if (worksheet == null)
                    throw new Exception("No worksheet found in the Excel file.");

                var rowCount = worksheet.Dimension.Rows;
                var technicians = new List<Technician>();

                for (int row = 2; row <= rowCount; row++) // Assuming first row contains headers
                {
                    var name = worksheet.Cells[row, 1].Text; // Column 1: Technician Name
                    var code = worksheet.Cells[row, 2].Text; // Column 2: Technician Code
                    var nickName = worksheet.Cells[row, 3].Text; // Column 3: Nickname
                    var nationalID = worksheet.Cells[row, 4].Text; // Column 4: National ID
                    var phoneNumberText = worksheet.Cells[row, 5].Text; // Column 5: Phone Numbers (comma-separated)
                    var governate = worksheet.Cells[row, 6].Text; // Column 6: Governate
                    var city = worksheet.Cells[row, 7].Text; // Column 7: City
                    var selectedCustomerCodes = worksheet.Cells[row, 8].Text; // Column 8: Customer Codes (comma-separated)
                    var selectedUsernames = worksheet.Cells[row, 9].Text; // Column 9: Usernames (comma-separated)

                    if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(code) && !string.IsNullOrWhiteSpace(phoneNumberText))
                    {
                        // Split the phone numbers
                        var phoneNumbers = phoneNumberText.Split(',')
                                                           .Select(p => p.Trim())
                                                           .ToList();

                        // Initialize phone number variables with default values
                        int phoneNumber1 = 0;
                        int? phoneNumber2 = null;
                        int? phoneNumber3 = null;

                        // Try parsing phone numbers to integers
                        if (phoneNumbers.Count > 0 && int.TryParse(phoneNumbers[0], out var parsedPhone1))
                            phoneNumber1 = parsedPhone1;

                        if (phoneNumbers.Count > 1 && int.TryParse(phoneNumbers[1], out var parsedPhone2))
                            phoneNumber2 = parsedPhone2;

                        if (phoneNumbers.Count > 2 && int.TryParse(phoneNumbers[2], out var parsedPhone3))
                            phoneNumber3 = parsedPhone3;

                        var technician = new Technician
                        {
                            Name = name,
                            Code = code,
                            NickName = nickName,
                            NationalID = nationalID,
                            PhoneNumber1 = phoneNumber1, // Assign first phone number
                            PhoneNumber2 = phoneNumber2, // Assign second phone number if available
                            PhoneNumber3 = phoneNumber3, // Assign third phone number if available
                            Governate = governate,
                            City = city,
                            IsActive = true,
                            CreatedAt = DateTime.Now,
                        };

                        // Split the customer codes and get customer IDs from the repository
                        var customerCodes = selectedCustomerCodes.Split(',').Select(c => c.Trim()).ToList();
                        var customerIds = await _customerRepo.GetCustomerIdsByCodesAsync(customerCodes);

                        // Add customers to the technician
                        technician.Customers = await _customerRepo.GetCustomersByIdsAsync(customerIds);

                        // Split the usernames and get user IDs from the UserManager
                        var usernames = selectedUsernames.Split(',').Select(c => c.Trim()).ToList();
                        var users = await _userManager.Users
                            .Where(u => usernames.Contains(u.UserName))
                            .ToListAsync();

                        // Assign users to the technician
                        technician.Users = users;

                        technicians.Add(technician);
                    }
                }

                // Save technicians to the database
                foreach (var technician in technicians)
                {
                    await _technicianRepo.AddAsync(technician);
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging (optional)
                Console.WriteLine(ex.Message);
                return false;
            }
        }




    }
}
