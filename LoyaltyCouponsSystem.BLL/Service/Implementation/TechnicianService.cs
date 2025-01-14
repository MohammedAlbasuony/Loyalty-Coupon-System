using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.BLL.ViewModel.Technician;
using LoyaltyCouponsSystem.DAL.Entity;
using LoyaltyCouponsSystem.DAL.Repo.Abstraction;
using Microsoft.AspNetCore.Mvc.Rendering;

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
                SelectedCity = technician.City
            }).ToList();

            foreach (var technicianViewModel in technicianViewModels)
            {
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
                        SelectedCity = technician.City
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
