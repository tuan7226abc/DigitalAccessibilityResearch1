
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DigitalAccessibilityResearch1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalAccessibilityResearch1.Controllers
{
    [Authorize]
    public class SurveyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SurveyController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(SurveyResponse response)
        {
            string? userId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            if (!ModelState.IsValid)
            {
                return View("Index", response);
            }

            response.Id = 0;
            response.UserId = userId;
            response.SubmittedAtUtc = DateTime.UtcNow;

            _context.SurveyResponses.Add(response);
            await _context.SaveChangesAsync();

            TempData["SurveyMessage"] =
                "Thank you. Your survey response has been submitted.";

            return RedirectToAction(nameof(Index));
        }
    }
}
