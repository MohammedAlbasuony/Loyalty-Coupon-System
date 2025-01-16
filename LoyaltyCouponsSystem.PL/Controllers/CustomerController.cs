using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.BLL.ViewModel.Customer;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyCouponsSystem.PL.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetAllCustomers()
        {
            var result = await _customerService.GetAllAsync();
            return View(result);
        }

        [HttpGet]
        public IActionResult AddCustomer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(CustomerViewModel customerViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _customerService.AddAsync(customerViewModel);
                if (result)
                {
                    return RedirectToAction("GetAllCustomers");
                }
                ModelState.AddModelError("", "Unable to add customer. Please try again.");
            }

            return View(customerViewModel);
        }

        public async Task<IActionResult> DeleteCustomer(string id)
        {
            await _customerService.DeleteAsync(id);
            return RedirectToAction(nameof(GetAllCustomers));
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCustomer(string id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            var updateCustomerViewModel = new UpdateCustomerViewModel
            {
                Name = customer.Name,
                Code = customer.Code,
                Governate = customer.Governate,
                City = customer.City,
                PhoneNumber = customer.PhoneNumber,
                TechnicianID = customer.TechnicianID
            };

            return View(updateCustomerViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCustomer(UpdateCustomerViewModel updateCustomerViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _customerService.UpdateAsync(updateCustomerViewModel);
                if (result)
                {
                    return RedirectToAction("GetAllCustomers");
                }
                ModelState.AddModelError("", "Failed to update customer");
            }

            return View(updateCustomerViewModel);
        }
    }
}
