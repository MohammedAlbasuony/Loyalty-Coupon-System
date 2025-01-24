using DocumentFormat.OpenXml.InkML;
using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.BLL.Service.Implementation;
using LoyaltyCouponsSystem.BLL.ViewModel.Technician;
using LoyaltyCouponsSystem.DAL.DB;
using LoyaltyCouponsSystem.DAL.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZXing;

namespace LoyaltyCouponsSystem.PL.Controllers
{
    public class TechnicianController : Controller
    {
        private readonly ITechnicianService _technicianService;
        private readonly ApplicationDbContext _DBcontext;

        public TechnicianController(ApplicationDbContext context, ITechnicianService technicianService)
        {
            _DBcontext = context;
            _technicianService = technicianService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetAllTechnicians()
        {
            // Fetch all technicians along with associated users and customers
            var technicians = await _technicianService.GetAllAsync();

            foreach (var technician in technicians)
            {
                // Ensure IsActive state is fetched correctly
                var dbTechnician = await _DBcontext.Technicians.FindAsync(technician.TechnicianID);
                if (dbTechnician != null)
                {
                    technician.IsActive = dbTechnician.IsActive;
                }

                technician.SelectedUserNames = technician.SelectedUserNames.Distinct().ToList();
                technician.SelectedCustomerNames = technician.SelectedCustomerNames.Distinct().ToList();
            }

            return View(technicians);
        }



        public async Task<IActionResult> GetTechnicianById(int id)
        {
            var result = await _technicianService.GetByIdAsync(id);
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> AddTechnician()
        {
            var technicianViewModel = new TechnicianViewModel
            {
                Customers = await _technicianService.GetCustomersForDropdownAsync(),
                Users = await _technicianService.GetUsersForDropdownAsync(),      
                Governates = new List<SelectListItem>
        {
                new SelectListItem { Text = "Cairo", Value = "Cairo" },
                new SelectListItem { Text = "Giza", Value = "Giza" },
                new SelectListItem { Text = "Alexandria", Value = "Alexandria" },
                new SelectListItem { Text = "Dakahlia", Value = "Dakahlia" },
                new SelectListItem { Text = "Red Sea", Value = "Red Sea" },
                new SelectListItem { Text = "Beheira", Value = "Beheira" },
                new SelectListItem { Text = "Fayoum", Value = "Fayoum" },
                new SelectListItem { Text = "Sharqia", Value = "Sharqia" },
                new SelectListItem { Text = "Aswan", Value = "Aswan" },
                new SelectListItem { Text = "Assiut", Value = "Assiut" },
                new SelectListItem { Text = "Beni Suef", Value = "Beni Suef" },
                new SelectListItem { Text = "Port Said", Value = "Port Said" },
                new SelectListItem { Text = "Damietta", Value = "Damietta" },
                new SelectListItem { Text = "Ismailia", Value = "Ismailia" },
                new SelectListItem { Text = "Kafr El Sheikh", Value = "Kafr El Sheikh" },
                new SelectListItem { Text = "Matruh", Value = "Matruh" },
                new SelectListItem { Text = "Luxor", Value = "Luxor" },
                new SelectListItem { Text = "Qalyubia", Value = "Qalyubia" },
                new SelectListItem { Text = "Qena", Value = "Qena" },
                new SelectListItem { Text = "Monufia", Value = "Monufia" },
                new SelectListItem { Text = "North Sinai", Value = "North Sinai" },
                new SelectListItem { Text = "Sohag", Value = "Sohag" },
                new SelectListItem { Text = "South Sinai", Value = "South Sinai" },
                new SelectListItem { Text = "New Valley", Value = "New Valley" },
                new SelectListItem { Text = "Gharbia", Value = "Gharbia" },
                new SelectListItem { Text = "Suez", Value = "Suez" }
        }
            };
            return View(technicianViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddTechnician(TechnicianViewModel technicianViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _technicianService.AddAsync(technicianViewModel);
                    if (result)
                    {
                        return RedirectToAction("GetAllTechnicians");
                    }
                    ModelState.AddModelError("", "Unable to add technician. Please try again.");
                }
                technicianViewModel.Customers = await _technicianService.GetCustomersForDropdownAsync();
                technicianViewModel.Users = await _technicianService.GetUsersForDropdownAsync();

                return View(technicianViewModel);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500);
            }
        }

        public async Task<IActionResult> DeleteTechnician(int id)
        {
            await _technicianService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> UpdateTechnician(int id)
        {
            var technician = await _technicianService.GetByIdAsync(id);

            if (technician == null)
            {
                return NotFound();
            }

            var model = new UpdateTechnicianViewModel
            {
                TechnicianID = technician.TechnicianID,
                Code = technician.Code,
                Name = technician.Name,
                NickName = technician.NickName,
                NationalID = technician.NationalID,
                SelectedGovernate = technician.SelectedGovernate,
                PhoneNumber1 = technician.PhoneNumber1,
                PhoneNumber2 = technician.PhoneNumber2,
                PhoneNumber3 = technician.PhoneNumber3,
                SelectedCity = technician.SelectedCity,
                Governates = GetGovernatesList() // Fetch governates dynamically
            }; return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateTechnician(UpdateTechnicianViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _technicianService.UpdateAsync(model);
                if (result)
                {
                    return RedirectToAction("GetAllTechnicians");
                }
                ModelState.AddModelError("", "Failed to update customer");
            }
            return View(model);
        }

        private List<SelectListItem> GetGovernatesList()
        {
            return new List<SelectListItem>
        {
            new SelectListItem { Value = "Cairo", Text = "Cairo" },
            new SelectListItem { Value = "Alexandria", Text = "Alexandria" },
            new SelectListItem { Value = "Giza", Text = "Giza" },
            new SelectListItem { Value = "Dakahlia", Text = "Dakahlia" },
            new SelectListItem { Value = "Red Sea", Text = "Red Sea" },
            new SelectListItem { Value = "Beheira", Text = "Beheira" },
            new SelectListItem { Value = "Fayoum", Text = "Fayoum" },
            new SelectListItem { Value = "Sharqia", Text = "Sharqia" },
            new SelectListItem { Value = "Aswan", Text = "Aswan" },
            new SelectListItem { Value = "Assiut", Text = "Assiut" },
            new SelectListItem { Value = "Beni Suef", Text = "Beni Suef" },
            new SelectListItem { Value = "Port Said", Text = "Port Said" },
            new SelectListItem { Value = "Suez", Text = "Suez" },
            new SelectListItem { Value = "Matruh", Text = "Matruh" },
            new SelectListItem { Value = "Qalyubia", Text = "Qalyubia" },
            new SelectListItem { Value = "Gharbia", Text = "Gharbia" },
            new SelectListItem { Value = "Monufia", Text = "Monufia" },
            new SelectListItem { Value = "Qena", Text = "Qena" },
            new SelectListItem { Value = "North Sinai", Text = "North Sinai" },
            new SelectListItem { Value = "Sohag", Text = "Sohag" },
            new SelectListItem { Value = "South Sinai", Text = "South Sinai" },
            new SelectListItem { Value = "Kafr El Sheikh", Text = "Kafr El Sheikh" },
            new SelectListItem { Value = "Damietta", Text = "Damietta" },
            new SelectListItem { Value = "Ismailia", Text = "Ismailia" },
            new SelectListItem { Value = "Luxor", Text = "Luxor" },
            new SelectListItem { Value = "New Valley", Text = "New Valley" }
        };
        }

        // Fetch cities based on the selected governate
        public IActionResult GetCitiesByGovernorate(string governorateId)
        {
            var citiesByGovernate = new Dictionary<string, List<string>>
        {
            { "Cairo", new List<string> { "Nasr City", "Heliopolis", "Maadi", "New Cairo" } },
            { "Alexandria", new List<string> { "Montaza", "Sidi Gaber", "Smouha" } },
            { "Giza", new List<string> { "Dokki", "Haram", "6th of October" } },
            { "Dakahlia", new List<string> { "Mansoura", "Mit Ghamr", "Talkha" } },
            { "Red Sea", new List<string> { "Hurghada", "Safaga", "El Qoseir" } },
            { "Beheira", new List<string> { "Damanhour", "Rashid", "Kafr El-Dawwar" } },
            { "Fayoum", new List<string> { "Fayoum City", "Tamiya", "Ibsheway" } },
            { "Sharqia", new List<string> { "Zagazig", "Belbeis", "10th of Ramadan City" } },
            { "Aswan", new List<string> { "Aswan City", "Kom Ombo", "Edfu" } },
            { "Assiut", new List<string> { "Assiut City", "Manfalut", "Dayrout" } },
            { "Beni Suef", new List<string> { "Beni Suef City", "Nasser", "Ihnasya" } },
            { "Port Said", new List<string> { "Port Said City" } },
            { "Suez", new List<string> { "Suez City" } },
            { "Matruh", new List<string> { "Marsa Matruh", "Siwa", "Alamein" } },
            { "Qalyubia", new List<string> { "Banha", "Shubra El-Kheima", "Qalyub" } },
            { "Gharbia", new List<string> { "Tanta", "El-Mahalla", "Kafr El-Zayat" } },
            { "Monufia", new List<string> { "Shebin El-Kom", "Sadat City", "Menouf" } },
            { "Qena", new List<string> { "Qena City", "Nag Hammadi", "Qus" } },
            { "North Sinai", new List<string> { "Arish", "Sheikh Zuweid", "Rafah" } },
            { "Sohag", new List<string> { "Sohag City", "Tahta", "Akhmim" } },
            { "South Sinai", new List<string> { "Sharm El-Sheikh", "Dahab", "Taba" } },
            { "Kafr El Sheikh", new List<string> { "Kafr El Sheikh City", "Desouk", "Balteem" } },
            { "Damietta", new List<string> { "Damietta City", "Ras El Bar", "New Damietta" } },
            { "Ismailia", new List<string> { "Ismailia City", "Fayed", "Qantara" } },
            { "Luxor", new List<string> { "Luxor City", "Armant", "Esna" } },
            { "New Valley", new List<string> { "Kharga", "Dakhla", "Farafra" } }
        };

            var cities = citiesByGovernate.ContainsKey(governorateId) ? citiesByGovernate[governorateId] : new List<string>();

            return Json(cities.Select(city => new { cityID = city, cityName = city }));
        }


        [HttpPost]
        public async Task<IActionResult> ToggleActivation(int technicianId, bool isActive)
        {
            var technician = await _DBcontext.Technicians.FindAsync(technicianId);
            if (technician == null)
            {
                return NotFound(new { success = false, message = "Technician not found" });
            }

            technician.IsActive = isActive;
            _DBcontext.Technicians.Update(technician);
            await _DBcontext.SaveChangesAsync();

            return Json(new { success = true });
        }
        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a valid Excel file.";
                return RedirectToAction("GetAllCustomers");
            }

            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);

                // Process the file in the service
                var result = await _technicianService.ImportTechniciansFromExcelAsync(stream);

                if (result)
                {
                    TempData["SuccessMessage"] = "technician imported successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to import technician. Please check the file format.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("GetAllTechnicians");
        }

    }
}

