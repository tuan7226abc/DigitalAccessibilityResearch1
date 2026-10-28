
using System.Security.Claims;
using DigitalAccessibilityResearch1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalAccessibilityResearch1.Controllers
{
    [Authorize]
    public class AccessibilityController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccessibilityController(ApplicationDbContext context)
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
        public async Task<IActionResult> CheckAnswer(string selectedAnswer)
        {
            if (string.IsNullOrWhiteSpace(selectedAnswer))
            {
                TempData["TaskMessage"] = "Please select an answer first.";
                TempData["TaskSuccess"] = false;
                return RedirectToAction(nameof(Index));
            }

            // Máy chủ tự kiểm tra đáp án.
            bool isCorrect = string.Equals(
                selectedAnswer,
                "Tab",
                StringComparison.OrdinalIgnoreCase);

            var attempt = new TaskAttempt
            {
                TaskName = "Keyboard Navigation",
                SelectedAnswer = selectedAnswer,
                IsCorrect = isCorrect,
                CompletedAtUtc = DateTime.UtcNow,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            _context.TaskAttempts.Add(attempt);
            await _context.SaveChangesAsync();

            TempData["TaskMessage"] = isCorrect
                ? "Correct! Your answer has been saved."
                : "Your answer has been saved. The correct answer is Tab.";

            TempData["TaskSuccess"] = isCorrect;

            return RedirectToAction(nameof(Index));
        }
    }
}
