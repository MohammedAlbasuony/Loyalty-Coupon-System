using DocumentFormat.OpenXml.InkML;
using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.BLL.ViewModel.Customer;
using LoyaltyCouponsSystem.DAL.DB;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyCouponsSystem.PL.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly ApplicationDbContext _DBcontext;

        public CustomerController(ApplicationDbContext context, ICustomerService customerService)
        {
            _DBcontext = context;
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

        public async Task<IActionResult> GetCustomerById(string id)
        {
            var result = await _customerService.GetByIdAsync(id);
            return View(result);
        }


        [HttpPost]
        public IActionResult ToggleActivation(string customerCode, [FromBody] bool isActive)
        {
            // Find customer in the database using the code
            var customer = _DBcontext.Customers.FirstOrDefault(c => c.Code == customerCode);
            if (customer == null)
            {
                return NotFound();
            }

            // Update the customer's active status
            customer.IsActive = isActive;
            _DBcontext.SaveChanges();

            return Ok();
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
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCustomer(string id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCustomer(CustomerViewModel customerViewModel)
        {
            if (ModelState.IsValid)
            {
                await _customerService.UpdateAsync(customerViewModel);
                return RedirectToAction("GetAllCustomers");
            }
            return View(customerViewModel);
        }
    }

}
