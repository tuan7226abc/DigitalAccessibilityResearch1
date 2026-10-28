
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using DigitalAccessibilityResearch1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalAccessibilityResearch1.Controllers
{
    [Authorize]
    public class LearningController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LearningController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Dữ liệu bài học mẫu.
        // Các bài học hiện được lưu trong code, chưa lưu trong SQLite.
        private static readonly List<Lesson> Lessons = new()
        {
            new Lesson
            {
                Id = 1,
                Title = "Introduction to Digital Accessibility",
                Description = "Learn what digital accessibility means and why it matters.",
                Category = "Accessibility Fundamentals",
                DurationMinutes = 10,

                Content = """
                    Digital accessibility means designing websites and digital
                    services so that people with different abilities can use them.

                    Accessible learning platforms help students perceive content,
                    navigate pages and complete learning activities.

                    Good accessibility can include readable text, sufficient
                    colour contrast, keyboard navigation, captions and clear
                    page structure.
                    """,

                LearningObjectives = new List<string>
                {
                    "Define digital accessibility.",
                    "Identify common accessibility barriers.",
                    "Explain why accessibility matters in digital learning."
                },

                Question = "Which option is an example of digital accessibility?",

                Options = new List<string>
                {
                    "Using very small text on every page",
                    "Allowing users to navigate using a keyboard",
                    "Using colour alone to communicate every message",
                    "Removing labels from form fields"
                },

                CorrectAnswer = "Allowing users to navigate using a keyboard",

                Explanation = "Keyboard navigation allows people to use a website without relying on a mouse."
            },

            new Lesson
            {
                Id = 2,
                Title = "Readable Text and Colour Contrast",
                Description = "Understand how text size and contrast affect readability.",
                Category = "Visual Accessibility",
                DurationMinutes = 8,

                Content = """
                    Text should be readable and sufficiently distinguishable
                    from its background.

                    Users may benefit from being able to enlarge text. Important
                    information should not depend on colour alone.

                    Designers should check contrast, text resizing and whether
                    content remains usable when text size changes.
                    """,

                LearningObjectives = new List<string>
                {
                    "Recognise poor text readability.",
                    "Understand the purpose of colour contrast.",
                    "Identify useful text-resizing features."
                },

                Question = "What is a useful accessibility feature for users who need larger text?",

                Options = new List<string>
                {
                    "A control to increase text size",
                    "A fixed text size that cannot change",
                    "Removing paragraph headings",
                    "Making the background and text identical in colour"
                },

                CorrectAnswer = "A control to increase text size",

                Explanation = "Text resizing helps users adjust the presentation to their reading needs."
            },

            new Lesson
            {
                Id = 3,
                Title = "Keyboard Navigation",
                Description = "Learn how to move through a website without a mouse.",
                Category = "Navigation Accessibility",
                DurationMinutes = 8,

                Content = """
                    Some users navigate websites with a keyboard instead of
                    a mouse.

                    The Tab key usually moves focus to the next interactive
                    element. Shift + Tab moves focus backwards. Enter or
                    Space activates many focused controls, depending on the
                    type of control.

                    A visible focus indicator helps users understand which
                    element is currently selected.
                    """,

                LearningObjectives = new List<string>
                {
                    "Use Tab and Shift + Tab to move between controls.",
                    "Recognise a visible keyboard focus indicator.",
                    "Identify common keyboard navigation barriers."
                },

                Question = "Which key usually moves keyboard focus to the next interactive element?",

                Options = new List<string>
                {
                    "Tab",
                    "Escape",
                    "Caps Lock",
                    "Print Screen"
                },

                CorrectAnswer = "Tab",

                Explanation = "Tab normally moves focus forward through interactive elements."
            }
        };

        // GET: /Learning
        // Hiển thị danh sách bài học.
        [HttpGet]
        public IActionResult Index()
        {
            return View(Lessons);
        }

        // GET: /Learning/Details/1
        // Hiển thị nội dung chi tiết của một bài học.
        [HttpGet]
        public IActionResult Details(int id)
        {
            Lesson? lesson = Lessons.FirstOrDefault(x => x.Id == id);

            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson);
        }

        // POST: /Learning/SubmitAnswer
        // Chấm câu trả lời tại server và lưu kết quả vào SQLite.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitAnswer(
            int lessonId,
            string? selectedAnswer)
        {
            // Kiểm tra bài học có tồn tại không.
            Lesson? lesson = Lessons.FirstOrDefault(x => x.Id == lessonId);

            if (lesson == null)
            {
                return NotFound();
            }

            // Kiểm tra người dùng đã chọn đáp án hợp lệ chưa.
            if (string.IsNullOrWhiteSpace(selectedAnswer) ||
                !lesson.Options.Contains(selectedAnswer))
            {
                TempData["QuizMessage"] =
                    "Please select a valid answer before submitting.";

                TempData["QuizSuccess"] = false;

                return RedirectToAction(
                    nameof(Details),
                    new { id = lessonId });
            }

            // Chấm điểm ở server, không tin kết quả từ trình duyệt.
            bool isCorrect = string.Equals(
                selectedAnswer,
                lesson.CorrectAnswer,
                StringComparison.Ordinal);

            // Tạo bản ghi kết quả.
            var attempt = new TaskAttempt
            {
                TaskName = lesson.Title,
                SelectedAnswer = selectedAnswer,
                IsCorrect = isCorrect,
                CompletedAtUtc = DateTime.UtcNow,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            // Lưu kết quả vào SQLite.
            _context.TaskAttempts.Add(attempt);

            await _context.SaveChangesAsync();

            // Thông báo sau khi lưu thành công.
            TempData["QuizMessage"] = isCorrect
                ? "Correct! Your result has been saved."
                : "Your result has been saved. Review the lesson and try again.";

            TempData["QuizSuccess"] = isCorrect;

            // Quay lại bài học.
            return RedirectToAction(
                nameof(Details),
                new { id = lessonId });
        }

        // GET: /Learning/MyLearning
        [HttpGet]
        public async Task<IActionResult> MyLearning()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var attempts = await _context.TaskAttempts
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CompletedAtUtc)
                .ToListAsync();

            return View(attempts);
        }
    }
}
