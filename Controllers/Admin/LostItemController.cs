// Controllers/Admin/LostItemController.cs
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using My_Uni_Hub.Models;
using My_Uni_Hub.Models.Pages;
using My_Uni_Hub.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace My_Uni_Hub.Controllers.Admin
{
    [Route("admin/lostitems")]
    public class LostItemController : Controller
    {
        private readonly MyUniDbContext _db;
        private readonly ILostItemService _service;
        private readonly IWebHostEnvironment _env;

        public LostItemController(MyUniDbContext db, ILostItemService service, IWebHostEnvironment env)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        // GET: /admin/lostitems
        [HttpGet("")]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var list = await _service.GetAllAsync(ct);
            return View( list);
        }

        // GET: /admin/lostitems/create
        [HttpGet("create")]
        public IActionResult Create() => View( new CreateAdminLostItemModel
        {
            Status = "lost"
        });

        // POST: /admin/lostitems/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(10_000_000)]
        public async Task<IActionResult> Create( [FromForm] CreateAdminLostItemModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View("Create", model);

            var entity = new LostItem
            {
                Name = model.Name,
                OwnerName = model.OwnerName,
                Description = model.Description,
                Location = model.Location,
                ContactInfo = model.ContactInfo,
                Status = model.Status,
                IsFound = model.Status == "found",
                PostedAt = DateTime.UtcNow,
                ImageUrl = "/images/lost-item.png"
            };

            await _service.CreateAsync(entity, ct);

            if (model.Image != null && model.Image.Length > 0)
            {
                try
                {
                    var url = await SaveImageForUser(model.Image);
                    entity.ImageUrl = url;
                    await _service.UpdateAsync(entity, ct);
                }
                catch { }
            }

            return RedirectToAction(nameof(Index));
        }


        private async Task<string> SaveImageForUser(IFormFile file)
        {
            var webRoot = _env.WebRootPath
                ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var uploadsDir = Path.Combine(webRoot, "uploads", "lostfound");
            Directory.CreateDirectory(uploadsDir);

            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid():N}{ext}";
            var filePath = Path.Combine(uploadsDir, fileName);

            using var fs = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(fs);

            return $"/uploads/lostfound/{fileName}";
        }



        // GET: /admin/lostitems/edit/{id}
        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var item = await _service.GetByIdAsync(id, ct);
            if (item == null) return NotFound();
            return View( item);
        }

        // POST: /admin/lostitems/edit/{id}
        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LostItem model, IFormFile? image, CancellationToken ct)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View( model);
            var existing = await _service.GetByIdAsync(id, ct);
            if (existing == null) return NotFound();

            model.ImageUrl = existing.ImageUrl;

            if (image != null && image.Length > 0)
            {
                var url = await SaveImage(image);
                model.ImageUrl = url;
            }

            model.Student = null;
            await _service.UpdateAsync(model, ct);
            return RedirectToAction("Index");
        }

        // POST: /admin/lostitems/delete
        [HttpPost("delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var exists = await _db.LostItems.AsNoTracking().AnyAsync(x => x.Id == id, ct);
            if (!exists) return NotFound();

            await _service.DeleteAsync(id, ct);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Ok(new { deleted = true, id });

            return RedirectToAction("Index");
        }

        private async Task<string> SaveImage(IFormFile file)
        {
            var uploadsDir = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
            if (!Directory.Exists(uploadsDir)) Directory.CreateDirectory(uploadsDir);

            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid():N}{ext}";
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }

            return $"/uploads/{fileName}";
        }//

        public class CreateAdminLostItemModel
        {
            [Required]
            public string Name { get; set; } = "";

            public string? OwnerName { get; set; }

            public string? Description { get; set; }

            public string? Location { get; set; }

            [Required]
            public string ContactInfo { get; set; } = "";

            [Required]
            public string Status { get; set; } = "lost";

            public IFormFile? Image { get; set; }
        }

    }
}
