using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.BLL.ViewModel.Customer;
using LoyaltyCouponsSystem.DAL.Entity;
using LoyaltyCouponsSystem.DAL.Repo.Abstraction;

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
                    Code = customerViewModel.Code,
                    Name = customerViewModel.Name,
                    ContactDetails = customerViewModel.ContactDetails
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
                Code = customer.Code,
                Name = customer.Name,
                ContactDetails = customer.ContactDetails
            }).ToList();

            return customerViewModels;
        }

        public async Task<CustomerViewModel> GetByIdAsync(string id)
        {
            if (!string.IsNullOrEmpty(id)) 
            {
                var customer = await _customerRepo.GetByIdAsync(id); 

                if (customer != null)
                {
                    var customerViewModel = new CustomerViewModel
                    {
                        Code = customer.Code,
                        Name = customer.Name,
                        ContactDetails = customer.ContactDetails
                    };

                    return customerViewModel;
                }
            }
            return null; 
        }


        public async Task<bool> UpdateAsync(CustomerViewModel customerViewModel)
        {
            if (customerViewModel != null)
            {
                // Manual Mapping
                var customer = new Customer
                {
                    Code = customerViewModel.Code,
                    Name = customerViewModel.Name,
                    ContactDetails = customerViewModel.ContactDetails
                };

                return await _customerRepo.UpdateAsync(customer);
            }
            return false;
        }
    }

}

