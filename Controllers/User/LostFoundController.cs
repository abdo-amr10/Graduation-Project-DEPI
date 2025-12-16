using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using My_Uni_Hub.Models.Pages;
using My_Uni_Hub.Services.Interfaces;

namespace My_Uni_Hub.Controllers.User
{
    [Authorize]
    [Route("lost-and-found")]
    public class LostFoundController : Controller
    {
        private readonly ILostItemService _service;
        private readonly IWebHostEnvironment _env;

        public LostFoundController(ILostItemService service, IWebHostEnvironment env)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        // GET /lost-and-found
        [HttpGet("")]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var items = await _service.GetAllAsync(ct);
            return View(items);
        }

        // POST /lost-and-found/create
        [HttpPost("create")]
        [RequestSizeLimit(10_000_000)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreateLostItemModel model, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                ModelState.AddModelError("type", "Please provide an item name.");
            if (string.IsNullOrWhiteSpace(model.ContactInfo))
                ModelState.AddModelError("contact", "Please provide contact info.");

            if (!ModelState.IsValid)
            {
                var items = await _service.GetAllAsync(ct);
                return View(items);
            }

            var entity = new LostItem
            {
                Name = model.Name,
                OwnerName = model.OwnerName,
                Description = model.Description ?? "",
                Location = model.Location,
                ContactInfo = model.ContactInfo,
                Status = model.Status ?? "lost",
                IsFound = string.Equals(model.Status, "found", StringComparison.OrdinalIgnoreCase),

                ImageUrl = "/images/lost-item.png"
            };

            if (model.Image != null && model.Image.Length > 0)
            {
                var ext = Path.GetExtension(model.Image.FileName) ?? "";
                var permitted = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                if (!permitted.Any(e => e.Equals(ext, StringComparison.OrdinalIgnoreCase)))
                {
                    ModelState.AddModelError("image", "Unsupported image type. Allowed: jpg,jpeg,png,gif,webp");
                    var items = await _service.GetAllAsync(ct);
                    return View(items);
                }

                var uploads = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "lostfound");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

                var fileName = $"{Guid.NewGuid():N}{ext}";
                var filePath = Path.Combine(uploads, fileName);
                try
                {
                    using (var fs = System.IO.File.Create(filePath))
                    {
                        await model.Image.CopyToAsync(fs, ct);
                    }
                    entity.ImageUrl = $"/uploads/lostfound/{fileName}";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Failed to save uploaded image: " + ex.Message);
                    var items = await _service.GetAllAsync(ct);
                    return View(items);
                }
            }

            try
            {
                await _service.CreateAsync(entity, ct);
                return RedirectToAction("Index");
            }
            catch (DbUpdateException dbEx)
            {
                ModelState.AddModelError("", "Database error: " + (dbEx.InnerException?.Message ?? dbEx.Message));
                var items = await _service.GetAllAsync(ct);
                return View(items);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unexpected error: " + ex.Message);
                var items = await _service.GetAllAsync(ct);
                return View(items);
            }
        }

    }

    public class CreateLostItemModel
    {
        [FromForm(Name = "type")]
        public string Name { get; set; } = "";

        [FromForm(Name = "owner")]
        public string? OwnerName { get; set; }

        [FromForm(Name = "description")]
        public string? Description { get; set; }

        [FromForm(Name = "place")]
        public string? Location { get; set; }

        [FromForm(Name = "contact")]
        public string? ContactInfo { get; set; }

        [FromForm(Name = "status")]
        public string? Status { get; set; }

        [FromForm(Name = "image")]
        public IFormFile? Image { get; set; }
    }
}