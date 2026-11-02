
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

        // Display survey form
        [HttpGet]
        public IActionResult Index()
        {
            return View(new SurveyResponse());
        }

        // Submit survey response
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(SurveyResponse response)
        {
            // Get the ID of the currently logged-in user
            string? userId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            // UserId is assigned by the server, not submitted by the form
            ModelState.Remove(nameof(SurveyResponse.UserId));

            // Check submitted data
            if (!ModelState.IsValid)
            {
                return View("Index", response);
            }

            // Prevent client-submitted values from being saved
            response.Id = 0;
            response.UserId = userId;
            response.SubmittedAtUtc = DateTime.UtcNow;

            // Save valid response to SQLite
            _context.SurveyResponses.Add(response);

            await _context.SaveChangesAsync();

            TempData["SurveyMessage"] =
                "Thank you. Your survey response has been submitted.";

            return RedirectToAction(nameof(Index));
        }
    }
}
