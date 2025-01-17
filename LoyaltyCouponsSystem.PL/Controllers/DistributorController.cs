using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.BLL.Service.Implementation;
using LoyaltyCouponsSystem.BLL.ViewModel.Distributor;
using LoyaltyCouponsSystem.BLL.ViewModel.Technician;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.PL.Controllers
{
    public class DistributorController : Controller
    {
        private readonly IDistributorService _distributorService;
        private readonly ICustomerService _customerService;
        public DistributorController(IDistributorService distributorService, ICustomerService customerService)
        {
            _distributorService = distributorService;
            _customerService = customerService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AddDistributor()
        {
            var viewModel = new DistributorViewModel
            {
                Governates = (List<SelectListItem>)await _distributorService.GetGovernatesForDropdownAsync(),
                Customers = await _distributorService.GetCustomersForDropdownAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddDistributor(DistributorViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Logic to handle distributor saving
                await _distributorService.AddAsync(model);
                return RedirectToAction("GetAllDistributors", "Distributor");
            }

            // In case of error, re-render the form with errors
            model.Governates = (List<SelectListItem>)await _distributorService.GetGovernatesForDropdownAsync();
            model.Customers = await _distributorService.GetCustomersForDropdownAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateDistributor(int id)
        {
            // Fetch distributor details by ID
            var distributorViewModel = await _distributorService.GetByIdAsync(id);
            if (distributorViewModel != null)
            {
                // Fetch the dropdown options for Governates and Customers
                distributorViewModel.Governates = (List<SelectListItem>)await _distributorService.GetGovernatesForDropdownAsync();
                distributorViewModel.Customers = await _distributorService.GetCustomersForDropdownAsync();
                return View(distributorViewModel); // Return the View with the populated ViewModel
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDistributor(UpdateVM DistributorViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _distributorService.UpdateAsync(DistributorViewModel);
                if (result)
                {
                    return RedirectToAction("GetAllDistributors"); // Redirect to list after successful update
                }

                ModelState.AddModelError("", "Unable to update distributor. Please try again.");
            }

            // If update fails, re-populate the dropdowns and return the view again
            DistributorViewModel.Customers = await _distributorService.GetCustomersForDropdownAsync();
            return View(DistributorViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDistributors()
        {
            
            var distributors = await _distributorService.GetAllAsync();
            return View(distributors);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDistributor(int id)
        {
            var result = await _distributorService.DeleteAsync(id);
            if (result)
            {
                return RedirectToAction("GetAllDistributors");
            }
            ModelState.AddModelError("", "Unable to delete distributor. Please try again.");
            return RedirectToAction("GetAllDistributors");
        }
    }
}
