using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Android.Models
{
    [Table("weight_entry")]
    public class WeightEntry
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [Column("weight")]
        public double Weight { get; set; }

        [Column("recorded_date")]
        public DateTime RecordedDate { get; set; }
    }
}