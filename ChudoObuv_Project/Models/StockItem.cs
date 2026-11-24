using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChudoObuv_Project.Models
{
    public class StockItem
    {
        [Key]
        public int Id { get; set; }

        public int Quantity { get; set; }

        [Column("Id_Product")]
        public int IdProduct { get; set; }

        [Column("Id_Size")]
        public int IdSize { get; set; }

        [ForeignKey("IdProduct")]
        [InverseProperty("StockItems")]
        public virtual Product IdProductNavigation { get; set; } = null!;

        [ForeignKey("IdSize")]
        [InverseProperty("StockItems")]
        public virtual Size IdSizeNavigation { get; set; } = null!;

        [InverseProperty("IdStockItemNavigation")]
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
