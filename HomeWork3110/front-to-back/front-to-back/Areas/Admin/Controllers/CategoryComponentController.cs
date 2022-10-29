using front_to_back.Areas.Admin.ViewModels.CategoryComponent;
using front_to_back.DAL;
using front_to_back.Helpers;
using front_to_back.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace front_to_back.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryComponentController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileService _fileService;

        public CategoryComponentController(AppDbContext appDbContext,IWebHostEnvironment webHostEnvironment,IFileService fileService)
        {
            _appDbContext = appDbContext;
            _webHostEnvironment = webHostEnvironment;
            _fileService = fileService;
        }
        public async Task<IActionResult> Index()
        {
            var model = new CategoryComponentIndexViewModel
            {
                CategoryComponents = await _appDbContext.CategoryComponents.Include(cc => cc.Category).ToListAsync()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new CategoryComponentCreateViewModel
            {
                Categories = await _appDbContext.Categories.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                }).ToListAsync()
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CategoryComponentCreateViewModel model)
        {
            model.Categories = await _appDbContext.Categories.Select(c => new SelectListItem
            {
                Text = c.Title,
                Value = c.Id.ToString()
            }).ToListAsync();

            if (!ModelState.IsValid) return View(model);

            var category = await _appDbContext.Categories.FindAsync(model.CategoryId);

            if (category==null)
            {
                ModelState.AddModelError("CategoryId", "Bu category movcud deyil");
                return View(model);
            }
            bool isExist = await _appDbContext.CategoryComponents.AnyAsync(cc => cc.Title.ToLower().Trim() == model.Title.ToLower().Trim());
            if (isExist)
            {
                ModelState.AddModelError("Title", "Bu adda category yoxdur");
                return View(model);
            }
            if (!_fileService.IsImage(model.Photo))
            {
                ModelState.AddModelError("Photo", "File image formatinda deyil zehmet olmasa image formasinda secin!!");
                return View(model);
            }
            var categoryComponent = new CategoryComponent
            {
                Title = model.Title,
                Description = model.Description,
                CategoryId = model.CategoryId,
                FilePath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath)
            };
            await _appDbContext.CategoryComponents.AddAsync(categoryComponent);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {

           

            var categoryComponent = await _appDbContext.CategoryComponents.FindAsync(id);
            if (categoryComponent == null) return NotFound();

            var model = new CategoryComponentDetailsViewModel
            {
                Title = categoryComponent.Title,
                Description=categoryComponent.Description,
                CategoryId=categoryComponent.CategoryId,
                FilePath=categoryComponent.FilePath,
                Categories = await _appDbContext.Categories.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                }).ToListAsync()
        };
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {

            var categoryComponent = await _appDbContext.CategoryComponents.FindAsync(id);
            if (categoryComponent == null) return NotFound();

            var model = new CategoryComponentUpdateViewModel
            {
                Title = categoryComponent.Title,
                Description = categoryComponent.Description,
                CategoryId = categoryComponent.CategoryId,
                FilePath = categoryComponent.FilePath,
                Categories = await _appDbContext.Categories.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                }).ToListAsync()
        };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult>Update(int id,CategoryComponentUpdateViewModel model)
        {
            model.Categories = await _appDbContext.Categories.Select(c => new SelectListItem
            {
                Text = c.Title,
                Value = c.Id.ToString()
            }).ToListAsync();
            if (!ModelState.IsValid) return View(model);

            if (id != model.Id) return BadRequest();

            var categoryComponent = await _appDbContext.CategoryComponents.FindAsync(id);
            if (categoryComponent == null) return NotFound();


            bool isExits = await _appDbContext.CategoryComponents.AnyAsync(cc => cc.Title.ToLower().Trim() == categoryComponent.Title.ToLower().Trim() && cc.Id != categoryComponent.Id);

            if (isExits)
            {
                ModelState.AddModelError("Title", "Bu kategoryComponentn movcuddur");
                return View(model);
            }
            categoryComponent.Title = model.Title;
            categoryComponent.Description = model.Description;

            if (model.Photo!=null)
            {
                if (!_fileService.IsImage(model.Photo))
                {
                    ModelState.AddModelError("Photo", "Image formatinda secin");
                    return View(model);
                }
                if (!_fileService.CheckSize(model.Photo,300))
                {
                    ModelState.AddModelError("Photo", "Sekilin olcusu 300 kb dan boyukdur");
                    return View(model);
                }

                _fileService.Delete(model.FilePath, _webHostEnvironment.WebRootPath);
               categoryComponent.FilePath=await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
            }
            var category = await _appDbContext.Categories.FindAsync(model.CategoryId);
            if (category == null) return NotFound();
            categoryComponent.CategoryId = category.Id;
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");
          
           
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {

            var categoryComponent = await _appDbContext.CategoryComponents.FindAsync(id);
            if (categoryComponent == null) return NotFound();


            _fileService.Delete(categoryComponent.FilePath, _webHostEnvironment.WebRootPath);

            _appDbContext.CategoryComponents.Remove(categoryComponent);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

      
       
    }
}
