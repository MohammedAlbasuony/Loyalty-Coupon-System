using DocumentFormat.OpenXml.Bibliography;
using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.BLL.ViewModel.Distributor;
using LoyaltyCouponsSystem.DAL.Entity;
using LoyaltyCouponsSystem.DAL.Repo.Abstraction;
using LoyaltyCouponsSystem.DAL.Repo.Implementation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Distributor = LoyaltyCouponsSystem.DAL.Entity.Distributor;

namespace LoyaltyCouponsSystem.BLL.Service.Implementation
{
    public class DistributorService : IDistributorService
    {
        private readonly IDistributorRepo _distributorRepo;
        private readonly ICustomerRepo _customerRepo;

        public DistributorService(IDistributorRepo distributorRepo, ICustomerRepo customerRepo)
        {
            _distributorRepo = distributorRepo;
            _customerRepo = customerRepo;
        }

        public async Task<bool> AddAsync(DistributorViewModel distributorViewModel)
        {

            if (distributorViewModel == null)
                return false;

            // Map Distributor entity
            var distributor = new Distributor
            {
                Name = distributorViewModel.Name,
                PhoneNumber1 = distributorViewModel.PhoneNumber1,
                Governate = distributorViewModel.SelectedGovernate,
                City = distributorViewModel.SelectedCity,
                Code = distributorViewModel.Code,
                IsDeleted = distributorViewModel.IsDeleted,
                CreatedBy = distributorViewModel.CreatedBy,
                CreatedAt = distributorViewModel.CreatedAt,
                DistributorCustomers = new List<DistributorCustomer>()
            };

            // Fetch Customer IDs from the repository based on selected customer codes
            var customerIds = await _customerRepo.GetCustomerIdsByCodesAsync(distributorViewModel.SelectedCustomerCodes);

            // Create DistributorCustomer mappings
            distributor.DistributorCustomers = customerIds
                .Select(customerId => new DistributorCustomer
                {
                    CustomerID = customerId
                })
                .ToList();

            return await _distributorRepo.AddAsync(distributor);
        }



        public async Task<bool> DeleteAsync(int id)
        {
            if (id > 0)
            {
                return await _distributorRepo.DeleteAsync(id);
            }
            return false;
        }

        public async Task<List<DistributorViewModel>> GetAllAsync()
        {
            var distributors = await _distributorRepo.GetAllAsync();
            return distributors.Select(d => new DistributorViewModel
            {
                DistributorID = d.DistributorID,
                Name = d.Name,
                PhoneNumber1 = d.PhoneNumber1,
                SelectedGovernate = d.Governate,
                SelectedCity = d.City,
                Code = d.Code,
                IsDeleted = d.IsDeleted,
                CreatedAt = d.CreatedAt,
                CreatedBy = d.CreatedBy,
                UpdatedBy = d.UpdatedBy,
                UpdatedAt = d.UpdatedAt,
                SelectedCustomerNames = d.DistributorCustomers.Where(dc => dc.Customer != null)
                .Select(x => x.Customer.Name)
                .Distinct().ToList()
            }).ToList();
        }

        public async Task<DistributorViewModel> GetByIdAsync(int id)
        {
            if (id != 0)
            {
                var distributor = await _distributorRepo.GetByIdAsync(id);
                if (distributor != null)
                {
                    var distributorCustomers = distributor.DistributorCustomers ?? new List<DistributorCustomer>();
                    return new DistributorViewModel
                    {
                        DistributorID = distributor.DistributorID,
                        Name = distributor.Name,
                        PhoneNumber1 = distributor.PhoneNumber1,
                        SelectedGovernate = distributor.Governate,
                        SelectedCity = distributor.City,
                        Code = distributor.Code,
                        IsDeleted = distributor.IsDeleted,
                        SelectedCustomerNames = distributorCustomers.Where(dc => dc.Customer != null)
                        .Select(x => x.Customer.Name)
                        .Distinct().ToList()
                    };
                }
            }
            return null;
        }


        public async Task<bool> UpdateAsync(UpdateVM distributorViewModel)
        {
            if (distributorViewModel != null)
            {
                // Fetch the existing distributor by ID to ensure it exists
                var existingDistributor = await _distributorRepo.GetByIdAsync(distributorViewModel.DistributorID);

                if (existingDistributor == null)
                {
                     return false;
                }

                // Map to Distributor entity for update
                var distributor = new Distributor
                {
                    DistributorID = distributorViewModel.DistributorID,
                    Name = distributorViewModel.Name,
                    PhoneNumber1 = distributorViewModel.PhoneNumber1,
                    Governate = distributorViewModel.SelectedGovernate,
                    City = distributorViewModel.SelectedCity,
                    Code = distributorViewModel.Code,
                    IsDeleted = distributorViewModel.IsDeleted,
                    UpdatedBy = distributorViewModel.UpdatedBy,
                    UpdatedAt = distributorViewModel.UpdatedAt
                };

                // Call the repository update method
                return await _distributorRepo.UpdateAsync(distributor);
            }
            return false;
        }


        // Fetch customers for dropdown
        public async Task<List<SelectListItem>> GetCustomersForDropdownAsync()
        {
            var customers = await _distributorRepo.GetCustomersForDropdownAsync();

            return customers.Select(c => new SelectListItem
            {
                Value = c.Code,
                Text = $"{c.Code} - {c.Name}"
            }).ToList();
        }
        public async Task<IEnumerable<SelectListItem>> GetGovernatesForDropdownAsync()
        {
            return new List<SelectListItem>
    {
        new SelectListItem { Text = "Cairo", Value = "Cairo" },
        new SelectListItem { Text = "Alexandria", Value = "Alexandria" },
        new SelectListItem { Text = "Giza", Value = "Giza" },
        new SelectListItem { Text = "Dakahlia", Value = "Dakahlia" },
        new SelectListItem { Text = "Red Sea", Value = "Red Sea" },
        new SelectListItem { Text = "Beheira", Value = "Beheira" },
        new SelectListItem { Text = "Fayoum", Value = "Fayoum" },
        new SelectListItem { Text = "Sharqia", Value = "Sharqia" },
        new SelectListItem { Text = "Aswan", Value = "Aswan" },
        new SelectListItem { Text = "Assiut", Value = "Assiut" },
        new SelectListItem { Text = "Beni Suef", Value = "Beni Suef" },
        new SelectListItem { Text = "Port Said", Value = "Port Said" },
        new SelectListItem { Text = "Suez", Value = "Suez" },
        new SelectListItem { Text = "Matruh", Value = "Matruh" },
        new SelectListItem { Text = "Qalyubia", Value = "Qalyubia" },
        new SelectListItem { Text = "Gharbia", Value = "Gharbia" },
        new SelectListItem { Text = "Monufia", Value = "Monufia" },
        new SelectListItem { Text = "Qena", Value = "Qena" },
        new SelectListItem { Text = "North Sinai", Value = "North Sinai" },
        new SelectListItem { Text = "Sohag", Value = "Sohag" },
        new SelectListItem { Text = "South Sinai", Value = "South Sinai" },
        new SelectListItem { Text = "Kafr El Sheikh", Value = "Kafr El Sheikh" },
        new SelectListItem { Text = "Damietta", Value = "Damietta" },
        new SelectListItem { Text = "Ismailia", Value = "Ismailia" },
        new SelectListItem { Text = "Luxor", Value = "Luxor" },
        new SelectListItem { Text = "New Valley", Value = "New Valley" },
        new SelectListItem { Text = "Sharkia", Value = "Sharkia" }
    };
        }

    }
}
