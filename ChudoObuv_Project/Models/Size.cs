using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChudoObuv_Project.Models
{
    public class Size
    {
        [Key]
        public int Id { get; set; }

        [StringLength(1024)]
        public string SizeValue { get; set; } = null!;

        [InverseProperty("IdSizeNavigation")]
        public virtual ICollection<StockItem> StockItems { get; set; } = new List<StockItem>();
    }
}
