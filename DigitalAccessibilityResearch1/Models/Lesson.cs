using System.Collections.Generic;

namespace DigitalAccessibilityResearch1.Models
{
    public class Lesson
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public int DurationMinutes { get; set; }

        public string Content { get; set; } = string.Empty;

        public List<string> LearningObjectives { get; set; } = new();

        public string Question { get; set; } = string.Empty;

        public List<string> Options { get; set; } = new();

        public string CorrectAnswer { get; set; } = string.Empty;

        public string Explanation { get; set; } = string.Empty;
    }
}