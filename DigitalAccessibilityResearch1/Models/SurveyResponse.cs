
using System;
using System.ComponentModel.DataAnnotations;

namespace DigitalAccessibilityResearch1.Models
{
    public class SurveyResponse
    {
        public int Id { get; set; }

        [Required]
        [StringLength(450)]
        public string UserId { get; set; } = string.Empty;

        [Range(1, 5, ErrorMessage = "Please select a rating from 1 to 5.")]
        public int EaseOfUse { get; set; }

        [Range(1, 5, ErrorMessage = "Please select a rating from 1 to 5.")]
        public int Readability { get; set; }

        [Range(1, 5, ErrorMessage = "Please select a rating from 1 to 5.")]
        public int Navigation { get; set; }

        [Range(1, 5, ErrorMessage = "Please select a rating from 1 to 5.")]
        public int TaskCompletion { get; set; }

        [StringLength(1000, ErrorMessage = "Comments cannot exceed 1000 characters.")]
        public string? Difficulties { get; set; }

        public DateTime SubmittedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
