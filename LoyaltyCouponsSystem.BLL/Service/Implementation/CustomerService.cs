using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.BLL.ViewModel.Customer;
using LoyaltyCouponsSystem.DAL.Entity;
using LoyaltyCouponsSystem.DAL.Repo.Abstraction;
using Microsoft.IdentityModel.Tokens;

namespace LoyaltyCouponsSystem.BLL.Service.Implementation
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepo _customerRepo;

        public CustomerService(ICustomerRepo customerRepo)
        {
            _customerRepo = customerRepo;
        }

        public async Task<bool> AddAsync(CustomerViewModel customerViewModel)
        {
            if (customerViewModel != null)
            {
                var customer = new Customer
                {
                    CustomerID = customerViewModel.CustomerID,
                    Name = customerViewModel.Name,
                    Code = customerViewModel.Code,
                    Governate = customerViewModel.Governate,
                    City = customerViewModel.City,
                    PhoneNumber = customerViewModel.PhoneNumber,
                    TechnicianId = customerViewModel.TechnicianID,
                    IsActive = customerViewModel.IsActive
                };
                return await _customerRepo.AddAsync(customer);
            }
            return false;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id != 0)
            {
                return await _customerRepo.DeleteAsync(id);
            }
            return false;
        }

        public async Task<List<CustomerViewModel>> GetAllAsync()
        {
            var customers = await _customerRepo.GetAllAsync();

            // Manual Mapping
            var customerViewModels = customers.Select(customer => new CustomerViewModel
            {
                CustomerID = customer.CustomerID,
                Name = customer.Name,
                Code = customer.Code,
                Governate = customer.Governate,
                City = customer.City,
                PhoneNumber = customer.PhoneNumber,
                CreatedBy = customer.CreatedBy,
                CreatedAt = customer.CreatedAt,
                UpdatedBy = customer.UpdatedBy,
                UpdatedAt = customer.UpdatedAt,
                IsActive = customer.IsActive,
            }).ToList();

            return customerViewModels;
        }
        // Get a customer by id
        public async Task<CustomerViewModel> GetByIdAsync(int id)
        {
            if (id != 0)
            {
                var customer = await _customerRepo.GetByIdAsync(id);

                if (customer != null)
                {
                    return new CustomerViewModel
                    {
                        CustomerID = customer.CustomerID,
                        Name = customer.Name,
                        Code = customer.Code,
                        Governate = customer.Governate,
                        City = customer.City,
                        PhoneNumber = customer.PhoneNumber,
                        TechnicianID = customer.TechnicianId,
                        IsActive = customer.IsActive
                    };
                }
            }
            return null;
        }

        public async Task<bool> UpdateAsync(UpdateCustomerViewModel updateCustomerViewModel)
        {
            if (updateCustomerViewModel != null && updateCustomerViewModel.CustomerID != 0) 
            {
                // Fetch the customer by ID
                var existingCustomer = await _customerRepo.GetByIdAsync(updateCustomerViewModel.CustomerID);

                if (existingCustomer != null)
                {
                    // Update properties
                    existingCustomer.Name = updateCustomerViewModel.Name;
                    existingCustomer.Code = updateCustomerViewModel.Code;  
                    existingCustomer.Governate = updateCustomerViewModel.Governate;
                    existingCustomer.City = updateCustomerViewModel.City;
                    existingCustomer.PhoneNumber = updateCustomerViewModel.PhoneNumber;
                    existingCustomer.TechnicianId = updateCustomerViewModel.TechnicianID;
                    existingCustomer.UpdatedAt = DateTime.Now;
                    existingCustomer.UpdatedBy = updateCustomerViewModel.UpdatedBy;
                    existingCustomer.IsActive = updateCustomerViewModel.IsActive;

                    return await _customerRepo.UpdateAsync(existingCustomer); 
                }
            }
            return false;
        }
        public async Task<bool> ToggleActivationAsync(int customerId, bool isActive)
        {
            var customer = await _customerRepo.GetByIdAsync(customerId);
            if (customer == null)
            {
                return false;
            }
            customer.IsActive = isActive;
            return await _customerRepo.UpdateAsync(customer);
        }

    }
}
