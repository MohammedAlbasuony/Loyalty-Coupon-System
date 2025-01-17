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




        public async Task<bool> DeleteAsync(string id)
        {
            if (!string.IsNullOrEmpty(id))
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


        public async Task<TechnicianViewModel> GetByIdAsync(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var technician = await _technicianRepo.GetByIdAsync(id);

                if (technician != null)
                {
                    var technicianViewModel = new TechnicianViewModel
                    {
                        Code = technician.Code,
                        Name = technician.Name,
                        NickName = technician.NickName,
                        NationalID = technician.NationalID,
                        PhoneNumber1 = technician.PhoneNumber1,
                        PhoneNumber2 = technician.PhoneNumber2,
                        PhoneNumber3 = technician.PhoneNumber3,
                        SelectedGovernate = technician.Governate,
                        SelectedCity = technician.City,
                        
                    };

                    PopulateDropdowns(technicianViewModel);
                    return technicianViewModel;
                }
            }
            return null;
        }

        public async Task<bool> UpdateAsync(TechnicianViewModel technicianViewModel)
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
                    City = technicianViewModel.SelectedCity
                };

                return await _technicianRepo.UpdateAsync(technician);
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
            // Fetch all users from the database
            var allUsers = await _userManager.Users.ToListAsync();

            // Filter the users asynchronously by role
            var representatives = new List<ApplicationUser>();
            foreach (var user in allUsers)
            {
                if (await _userManager.IsInRoleAsync(user, "Representative"))
                {
                    representatives.Add(user);
                }
            }
            return representatives.Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = $"{u.UserName}" 
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
    }
}
