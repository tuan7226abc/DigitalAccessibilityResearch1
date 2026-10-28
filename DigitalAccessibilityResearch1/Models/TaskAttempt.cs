using System.ComponentModel.DataAnnotations;

namespace DigitalAccessibilityResearch1.Models
{
    public class TaskAttempt
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string TaskName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string SelectedAnswer { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }

        public DateTime CompletedAtUtc { get; set; } = DateTime.UtcNow;

        // Liên kết với tài khoản đang đăng nhập.
        // Dùng ID thay vì lưu email trực tiếp.
        [StringLength(450)]
        public string? UserId { get; set; }
    }
}