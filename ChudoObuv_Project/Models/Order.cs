using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChudoObuv_Project.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime OrderDate { get; set; }

        public int Quantity { get; set; }

        public int PricePerUnit { get; set; }

        [Column("Id_User")]
        public int IdUser { get; set; }

        [Column("Id_StockItem")]
        public int IdStockItem { get; set; }

        [ForeignKey("IdUser")]
        [InverseProperty("Orders")]
        public virtual User IdUserNavigation { get; set; } = null!;

        [ForeignKey("IdStockItem")]
        [InverseProperty("Orders")]
        public virtual StockItem IdStockItemNavigation { get; set; } = null!;
    }
}
