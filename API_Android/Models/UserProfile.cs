using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Android.Models
{
    [Table("user_profile")]
    public class UserProfile
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [Column("name")]
        public string? Name { get; set; }

        [Column("age")]
        public int? Age { get; set; }

        [Column("gender")]
        public string? Gender { get; set; }

        [Column("height")]
        public double? Height { get; set; } 

        [Column("target_weight")]
        public double? TargetWeight { get; set; } 

        [Column("activity_level")]
        public string? ActivityLevel { get; set; } 
    }
}