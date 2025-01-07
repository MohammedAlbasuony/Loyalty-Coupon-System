using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.BLL.ViewModel.Customer;
using LoyaltyCouponsSystem.BLL.ViewModel.Technician;
using LoyaltyCouponsSystem.DAL.Entity;
using LoyaltyCouponsSystem.DAL.Repo.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.BLL.Service.Implementation
{
    public class TechnicianService : ITechnicianService
    {
        private readonly ITechnicianRepo _technicianRepo;

        public TechnicianService(ITechnicianRepo technicianRepo)
        {
            _technicianRepo = technicianRepo;
        }

        public async Task<bool> AddAsync(TechnicianViewModel technicianViewModel)
        {
            if (technicianViewModel != null)
            {

                var technician = new Technician
                {
                    Code = technicianViewModel.Code,
                    Name = technicianViewModel.Name,
                    ContactDetails = technicianViewModel.ContactDetails
                };

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

            // Manual Mapping
            var technicianViewModel = technicians.Select(technician => new TechnicianViewModel
            {
                Code = technician.Code,
                Name = technician.Name,
                ContactDetails = technician.ContactDetails
            }).ToList();

            return technicianViewModel;
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
                        ContactDetails = technician.ContactDetails
                    };

                    return technicianViewModel;
                }
            }
            return null;
        }


        public async Task<bool> UpdateAsync(TechnicianViewModel technicianViewModel)
        {
            if (technicianViewModel != null)
            {
                // Manual Mapping
                var technician = new Technician
                {
                    Code = technicianViewModel.Code,
                    Name = technicianViewModel.Name,
                    ContactDetails = technicianViewModel.ContactDetails
                };

                return await _technicianRepo.UpdateAsync(technician);
            }
            return false;
        }
    }
}
