using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.BLL.ViewModel.Customer;
using LoyaltyCouponsSystem.BLL.ViewModel.Technician;
using LoyaltyCouponsSystem.DAL.Entity;
using LoyaltyCouponsSystem.DAL.Repo.Abstraction;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyCouponsSystem.BLL.Service.Implementation
{
    public class ExchangeOrderService : IExchangeOrderService
    {
        private readonly IExchangeOrderRepo _repository;

        public ExchangeOrderService(IExchangeOrderRepo repository)
        {
            _repository = repository;
        }

        public async Task<CustomerViewModel> GetCustomerDetailsAsync(string customerCodeOrName)
        {
            var customer = await _repository.GetCustomerByCodeOrNameAsync(customerCodeOrName);

            if (customer == null)
                return null;

            return new CustomerViewModel
            {
                Name = customer.Name,
                Code = customer.Code,
                Governate = customer.Governate,
                City = customer.City,
                PhoneNumber = customer.PhoneNumber
            };
        }

        public async Task<TechnicianViewModel> GetTechnicianDetailsAsync(string technicianCodeOrName)
        {
            var technician = await _repository.GetTechnicianByCodeOrNameAsync(technicianCodeOrName);

            if (technician == null)
                return null;

            return new TechnicianViewModel
            {
                Code = technician.Code,
                Name = technician.Name
            };
        }

        public async Task<AssignmentViewModel> GetAssignmentDetailsAsync()
        {
            var customers = await _repository.GetAllCustomersAsync();
            var technicians = await _repository.GetAllTechniciansAsync();

            return new AssignmentViewModel
            {
                Customers = customers.Select(c => new SelectListItem
                {
                    Value = c.Code,
                    Text = $"{c.Name} ({c.Code})"
                }).ToList(),

                Technicians = technicians.Select(t => new SelectListItem
                {
                    Value = t.Code,
                    Text = $"{t.Name} ({t.Code})"
                }).ToList(),

                Governates = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Cairo", Text = "Cairo" },
                    new SelectListItem { Value = "Gharbeia", Text = "Gharbeia" },
                    new SelectListItem { Value = "Sharqeia", Text = "Sharqeia" },
                    new SelectListItem { Value = "Alexandria", Text = "Alexandria" }
                },

                CouponSorts = new List<SelectListItem>
                {
                    new SelectListItem { Value = "اختبار", Text = "كعب إختبار" },
                    new SelectListItem { Value = "عادي", Text = "كوبون عادي" },
                    new SelectListItem { Value = "حافز", Text = "كوبون حافز" },
                    new SelectListItem { Value = "مرتجع", Text = "كوبون مرتجع" }
                },
                CouponTypes = new List<SelectListItem>
                {
                    new SelectListItem { Value = "راك ثيرم", Text = "راك ثيرم" },
                    new SelectListItem { Value = "صرف جي تكس", Text = "صرف جي تكس" },
                    new SelectListItem { Value = "اقطار كبيرة وهودذا", Text = "اقطار كبيرة وهودذا" },
                    new SelectListItem { Value = "كعب راك ثيرم", Text = "كعب راك ثيرم" },
                    new SelectListItem { Value = "كعب صرف جي تكس", Text = "كعب صرف جي تكس" },
                    new SelectListItem { Value = "كعب اقطار كبيرة وهودذا", Text = "كعب اقطار كبيرة وهودذا" }
                }
            };
        }

        public async Task AssignQRCodeAsync(string selectedCustomerCode, string selectedTechnicianCode, string selectedGovernate, string selectedCity, List<AssignmentViewModel> transactions)
        {
            if (transactions == null || !transactions.Any())
                throw new ArgumentException("Transactions cannot be null or empty.");

            var customer = await _repository.GetCustomerByCodeOrNameAsync(selectedCustomerCode);
            var technician = await _repository.GetTechnicianByCodeOrNameAsync(selectedTechnicianCode);

            if (customer == null)
                throw new ArgumentException("Invalid customer code.");

            if (technician == null)
                throw new ArgumentException("Invalid technician code.");

            string exchangePermissionNum = transactions[0].ExchangePermission;

            foreach (var transaction in transactions)
            {
                if (await IsExchangePermissionDuplicateAsync(transaction.ExchangePermission))
                {
                    throw new InvalidOperationException($"Exchange Permission Number {transaction.ExchangePermission} is already used.");
                }

                if (transaction.StartSequenceNumber > transaction.EndSequenceNumber)
                    throw new ArgumentException("Start sequence number cannot be greater than end sequence number.");

                for (long seqNum = transaction.StartSequenceNumber; seqNum <= transaction.EndSequenceNumber; seqNum++)
                {
                    var exists = await _repository.TransactionExistsAsync(exchangePermissionNum, seqNum);
                    if (exists)
                        throw new InvalidOperationException($"Sequence number {seqNum} already exists for Exchange Permission {exchangePermissionNum}.");

                    var newTransaction = new Transaction
                    {
                        CustomerID = customer.CustomerID,
                        TechnicianID = technician.TechnicianID,
                        Governate = selectedGovernate,
                        City = selectedCity,
                        Timestamp = DateTime.Now,
                        CouponSort = transaction.SelectedCouponSort,
                        CouponType = transaction.SelectedCouponType,
                        SequenceNumber = seqNum,
                        ExchangePermission = exchangePermissionNum,
                        CreatedBy = transaction.CreatedBy,
                        SequenceStart = transaction.StartSequenceNumber.ToString(),
                        SequenceEnd = transaction.EndSequenceNumber.ToString(),
                    };

                    await _repository.AddTransactionAsync(newTransaction);
                }
            }

            await _repository.SaveChangesAsync();
        }

        public async Task<bool> IsExchangePermissionDuplicateAsync(string exchangePermission)
        {
            return await _repository.ExchangePermissionExistsAsync(exchangePermission);
        }
    }
}
