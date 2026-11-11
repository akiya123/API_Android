using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Android.Models
{
    [Table("water_log")]
    public class WaterLog
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [Column("amount_ml")]
        public int AmountMl { get; set; }

        [Column("logged_at")]
        public DateTime LoggedAt { get; set; }
    }
}