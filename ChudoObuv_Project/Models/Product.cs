using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChudoObuv_Project.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [StringLength(1024)]
        public string ProductName { get; set; } = null!;

        [StringLength(1024)]
        public string? Description { get; set; }

        [StringLength(1024)]
        public string? Composition { get; set; }

        public int Price { get; set; }

        [StringLength(1024)]
        public string? ImagePath { get; set; }

        [Column("Id_Category")]
        public int IdCategory { get; set; }

        [Column("Id_Subcategory")]
        public int IdSubcategory { get; set; }

        [Column("Id_Manufacturer")]
        public int IdManufacturer { get; set; }

        [ForeignKey("IdCategory")]
        [InverseProperty("Products")]
        public virtual Category IdCategoryNavigation { get; set; } = null!;

        [ForeignKey("IdSubcategory")]
        [InverseProperty("Products")]
        public virtual Subcategory IdSubcategoryNavigation { get; set; } = null!;

        [ForeignKey("IdManufacturer")]
        [InverseProperty("Products")]
        public virtual Manufacturer IdManufacturerNavigation { get; set; } = null!;

        [InverseProperty("IdProductNavigation")]
        public virtual ICollection<StockItem> StockItems { get; set; } = new List<StockItem>();
    }
}
