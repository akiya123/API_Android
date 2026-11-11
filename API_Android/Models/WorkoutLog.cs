using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Android.Models
{
    [Table("workout_log")]
    public class WorkoutLog
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [Column("exercise_name")]
        public string ExerciseName { get; set; } = string.Empty;

        [Column("duration_minutes")]
        public int DurationMinutes { get; set; }

        [Column("calories_burned")]
        public int? CaloriesBurned { get; set; }

        [Column("logged_at")]
        public DateTime LoggedAt { get; set; }
    }
}