using front_to_back.Areas.Admin.ViewModels;
using front_to_back.Areas.Admin.ViewModels.Contact;
using front_to_back.DAL;
using front_to_back.Helpers;
using front_to_back.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace front_to_back.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContactController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileService _fileService;

        public ContactController(AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment, IFileService fileService)
        {
            _appDbContext = appDbContext;
            _webHostEnvironment = webHostEnvironment;
            _fileService = fileService;
        }
        public async Task<IActionResult> Index()
        {
            var model = new ContactIndexViewModel
            {
                ContactBannerHeroe = await _appDbContext.ContactBannerHeroes.ToListAsync()
            };
            return View(model);
        }
        #region Create

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Create(ContactBannerHero contactBanner)
        {
            if (!ModelState.IsValid) return View(contactBanner);
            if (!_fileService.IsImage(contactBanner.Photo))
            {
                ModelState.AddModelError("Photo", "Yüklənən fayl image formatında olmalıdır.");
                return View(contactBanner);
            }

            int maxSize = 100;

            if (!_fileService.CheckSize(contactBanner.Photo, maxSize))
            {
                ModelState.AddModelError("Photo", $"Şəkilin ölçüsü {maxSize} kb-dan böyükdür");
                return View(contactBanner);
            }



            contactBanner.PhotoPath = await _fileService.UploadAsync(contactBanner.Photo, _webHostEnvironment.WebRootPath);
            await _appDbContext.ContactBannerHeroes.AddAsync(contactBanner);


            await _appDbContext.SaveChangesAsync();


            return RedirectToAction("Index");
        }
        #endregion
        #region Details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var contactBanner = await _appDbContext.ContactBannerHeroes.FindAsync(id);
            if (contactBanner == null) return NotFound();

            return View(contactBanner);

        }
        #endregion
        #region Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var contactBanner = await _appDbContext.ContactBannerHeroes.FindAsync(id);
            if (contactBanner == null) return NotFound();
            return View(contactBanner);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteComponent(int id)
        {

            var contactBanner = await _appDbContext.ContactBannerHeroes.FindAsync(id);
            if (contactBanner == null) return NotFound();

            _fileService.Delete(contactBanner.PhotoPath, _webHostEnvironment.WebRootPath);

            _appDbContext.ContactBannerHeroes.Remove(contactBanner);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");

        }
        #endregion

        #region Update
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var contactBanner = await _appDbContext.ContactBannerHeroes.FindAsync(id);
            if (contactBanner == null) return NotFound();

            var model = new ContactIndexUpdateViewModel
            {
                Id = contactBanner.Id,
                Title = contactBanner.Title,
                Title2 = contactBanner.Title2,
                Information = contactBanner.Information,
                PhotoPath = contactBanner.PhotoPath
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, ContactIndexUpdateViewModel contactIndexUpdateView)
        {
            if (!ModelState.IsValid) return View(contactIndexUpdateView);
            if (id != contactIndexUpdateView.Id) return BadRequest();

            var contactBanner = await _appDbContext.ContactBannerHeroes.FindAsync(contactIndexUpdateView.Id);
            if (contactBanner is null) return NotFound();

            contactBanner.Title = contactIndexUpdateView.Title;
            contactBanner.Title2 = contactIndexUpdateView.Title2;
            contactBanner.Information = contactIndexUpdateView.Information;

            if (!_fileService.IsImage(contactIndexUpdateView.Photo))
            {
                ModelState.AddModelError("Photo", "Yüklənən fayl image formatında olmalıdır.");
                return View(contactIndexUpdateView);
            }

            int maxSize = 100;

            if (!_fileService.CheckSize(contactIndexUpdateView.Photo, maxSize))
            {
                ModelState.AddModelError("Photo", $"Şəkilin ölçüsü {maxSize} kb-dan böyükdür");
                return View(contactIndexUpdateView);
            }

            if (contactIndexUpdateView.Photo != null)
            {
                _fileService.Delete(contactBanner.PhotoPath,_webHostEnvironment.WebRootPath);

                contactBanner.PhotoPath = await _fileService.UploadAsync(contactIndexUpdateView.Photo, _webHostEnvironment.WebRootPath);
            }



            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #endregion
    }
}
