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
                    TechnicianId = customerViewModel.TechnicianID
                };
                return await _customerRepo.AddAsync(customer);
            }
            return false;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            if (!string.IsNullOrEmpty(id))
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
                CreatedAt = customer.CreatedAt
            }).ToList();

            return customerViewModels;
        }
        // Get a customer by code
        public async Task<CustomerViewModel> GetByIdAsync(string id)
        {
            if (!string.IsNullOrEmpty(id))
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
                        TechnicianID = customer.TechnicianId
                    };
                }
            }
            return null;
        }
        


        public async Task<bool> UpdateAsync(UpdateCustomerViewModel updateCustomerViewModel)
        {
            if (updateCustomerViewModel != null && !string.IsNullOrEmpty(updateCustomerViewModel.Code)) 
            {
                // Fetch the customer using the new identifier (CustomerID)
                var existingCustomer = await _customerRepo.GetByIdAsync(updateCustomerViewModel.Code);
                if (existingCustomer != null)
                {
                    existingCustomer.Name = updateCustomerViewModel.Name;
                    existingCustomer.Code = updateCustomerViewModel.Code;  // This is still updated, but it's not the identifier
                    existingCustomer.Governate = updateCustomerViewModel.Governate;
                    existingCustomer.City = updateCustomerViewModel.City;
                    existingCustomer.PhoneNumber = updateCustomerViewModel.PhoneNumber;
                    existingCustomer.TechnicianId = updateCustomerViewModel.TechnicianID;

                    return await _customerRepo.UpdateAsync(existingCustomer);
                }
            }
            return false;
        }



    }
}
