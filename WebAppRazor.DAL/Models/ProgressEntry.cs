using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppRazor.DAL.Models
{
    public class ProgressEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        public double Weight { get; set; }

        // Recalculated based on latest height from profile
        public double BMI { get; set; }
        public double BMR { get; set; }
        public double TDEE { get; set; }

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;

        public DateTime RecordedAt { get; set; } = DateTime.Now;
    }
}
