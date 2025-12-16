using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using My_Uni_Hub.Models.Pages;
using My_Uni_Hub.Services.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace My_Uni_Hub.Controllers.User
{
    [Authorize]
    [Route("opportunities")]
    public class UserOpportunityController : Controller
    {
        private readonly IOpportunityService _service;

        public UserOpportunityController(IOpportunityService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        // GET /opportunities
        [HttpGet("")]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var items = await _service.GetAllAsync(ct);
            return View( items);
        }

        // GET /opportunities/data  (optional JSON endpoint for client fetch)
        [HttpGet("data")]
        public async Task<IActionResult> Data(CancellationToken ct)
        {
            var items = await _service.GetAllAsync(ct);
            return Ok(items);
        }

        // GET /opportunities/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var item = await _service.GetByIdAsync(id, ct);
            if (item == null) return NotFound();
            return Ok(item);
        }
    }
}
