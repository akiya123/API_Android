using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Android.Models
{
    [Table("food_log")]
    public class FoodLog
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [Column("food_name")]
        public string FoodName { get; set; } = string.Empty;

        [Column("calories")]
        public int Calories { get; set; }

        [Column("protein")]
        public double? Protein { get; set; } 

        [Column("carbs")]
        public double? Carbs { get; set; } 

        [Column("fat")]
        public double? Fat { get; set; } 

        [Column("logged_at")]
        public DateTime LoggedAt { get; set; }
    }
}