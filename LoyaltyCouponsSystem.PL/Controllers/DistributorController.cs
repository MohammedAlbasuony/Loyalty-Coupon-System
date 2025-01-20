using DocumentFormat.OpenXml.InkML;
using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.BLL.ViewModel.Distributor;
using LoyaltyCouponsSystem.DAL.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.PL.Controllers
{
    public class DistributorController : Controller
    {
        private readonly IDistributorService _distributorService;
        private readonly ICustomerService _customerService;
        private readonly ApplicationDbContext _DBcontext;

        public DistributorController(ApplicationDbContext context, IDistributorService distributorService, ICustomerService customerService)
        {
            _DBcontext = context;
            _distributorService = distributorService;
            _customerService = customerService;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Add Distributor (GET)
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

        // Add Distributor (POST)
        [HttpPost]
        public async Task<IActionResult> AddDistributor(DistributorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _distributorService.AddAsync(model);
                if (result)
                {
                    return RedirectToAction("GetAllDistributors", "Distributor");
                }

                // If Add fails, show error and re-render
                ModelState.AddModelError("", "Unable to add distributor. Please try again.");
            }

            // If validation fails, re-populate dropdowns and return the view
            model.Governates = (List<SelectListItem>)await _distributorService.GetGovernatesForDropdownAsync();
            model.Customers = await _distributorService.GetCustomersForDropdownAsync();
            return View(model);
        }

        // Update Distributor (GET)
        [HttpGet]
        public async Task<IActionResult> UpdateDistributor(int id)
        {
            // Validate the id
            if (id <= 0)
            {
                return NotFound();
            }

            var distributorViewModel = await _distributorService.GetByIdAsync(id);
            if (distributorViewModel == null)
            {
                return NotFound();
            }

            distributorViewModel.Governates = (List<SelectListItem>)await _distributorService.GetGovernatesForDropdownAsync();
            distributorViewModel.Customers = await _distributorService.GetCustomersForDropdownAsync();

            return View(distributorViewModel); // Return populated ViewModel to the view
        }

        // Update Distributor (POST)
        [HttpPost]
        public async Task<IActionResult> UpdateDistributor(UpdateVM distributorViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _distributorService.UpdateAsync(distributorViewModel);
                if (result)
                {
                    return RedirectToAction("GetAllDistributors"); // Redirect to list after successful update
                }

                ModelState.AddModelError("", "Unable to update distributor. Please try again.");
            }

            // If update fails, re-populate dropdowns and return the view again
            distributorViewModel.Customers = await _distributorService.GetCustomersForDropdownAsync();
            return View(distributorViewModel);
        }

        // Get All Distributors
        [HttpGet]
        public async Task<IActionResult> GetAllDistributors()
        {
            var distributors = await _distributorService.GetAllAsync();
            return View(distributors);
        }

        // Delete Distributor (POST)
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

        [HttpPost]
        public async Task<IActionResult> ToggleActivation(int distributorId)
        {
            var distributor = await _DBcontext.Distributors.FindAsync(distributorId);
            if (distributor == null)
            {
                return Json(new { success = false, message = "Distributor not found." });
            }

            distributor.IsActive = !distributor.IsActive;
            distributor.UpdatedAt = DateTime.Now;
            distributor.UpdatedBy = User.Identity.Name;
            await _DBcontext.SaveChangesAsync();

            return Json(new { success = true, isActive = distributor.IsActive });
        }

    }
}
