using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.BLL.ViewModel.Distributor;
using LoyaltyCouponsSystem.DAL.Entity;
using LoyaltyCouponsSystem.DAL.Repo.Abstraction;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.BLL.Service.Implementation
{
    public class DistributorService : IDistributorService
    {
        private readonly IDistributorRepo _distributorRepo;

        public DistributorService(IDistributorRepo distributorRepo)
        {
            _distributorRepo = distributorRepo;
        }

        public async Task<bool> AddAsync(DistributorViewModel distributorViewModel)
        {

            if (distributorViewModel != null)
            {
                var distributor = new Distributor
                {
                    Name = distributorViewModel.Name,
                    PhoneNumber1 = distributorViewModel.PhoneNumber1,
                    Governate = distributorViewModel.SelectedGovernate,
                    City = distributorViewModel.SelectedCity,
                    Code = distributorViewModel.Code,
                    IsDeleted = distributorViewModel.IsDeleted
                };

                return await _distributorRepo.AddAsync(distributor);
            }
            return false;
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
                IsDeleted = d.IsDeleted
            }).ToList();
        }

        public async Task<DistributorViewModel> GetByIdAsync(int id)
        {
            if (id > 0)
            {
                var distributor = await _distributorRepo.GetByIdAsync(id);
                if (distributor != null)
                {
                    return new DistributorViewModel
                    {
                        DistributorID = distributor.DistributorID,
                        Name = distributor.Name,
                        PhoneNumber1 = distributor.PhoneNumber1,
                        SelectedGovernate = distributor.Governate,
                        SelectedCity = distributor.City,
                        Code = distributor.Code,
                        IsDeleted = distributor.IsDeleted
                    };
                }
            }
            return null;
        }

        public async Task<bool> UpdateAsync(DistributorViewModel distributorViewModel)
        {
            if (distributorViewModel != null)
            {
                var distributor = new Distributor
                {
                    DistributorID = distributorViewModel.DistributorID,
                    Name = distributorViewModel.Name,
                    PhoneNumber1 = distributorViewModel.PhoneNumber1,
                    Governate = distributorViewModel.SelectedGovernate,
                    City = distributorViewModel.SelectedCity,
                    Code = distributorViewModel.Code,
                    IsDeleted = distributorViewModel.IsDeleted
                };

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
    }
}
