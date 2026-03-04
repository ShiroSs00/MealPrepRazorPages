using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppRazor.DAL.Models
{
    public class MealReview
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [Required]
        public int MealItemId { get; set; }

        [ForeignKey("MealItemId")]
        public MealItem MealItem { get; set; } = null!;

        // 1-5 stars
        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string Comment { get; set; } = string.Empty;

        // Points earned for this review
        public int PointsEarned { get; set; } = 10;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
