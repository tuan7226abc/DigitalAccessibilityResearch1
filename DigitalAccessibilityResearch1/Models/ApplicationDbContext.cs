
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DigitalAccessibilityResearch1.Models
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TaskAttempt> TaskAttempts => Set<TaskAttempt>();

        public DbSet<SurveyResponse> SurveyResponses => Set<SurveyResponse>();
    }
}
