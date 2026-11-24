using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChudoObuv_Project.Models
{
    public class Subcategory
    {
        [Key]
        public int Id { get; set; }

        [StringLength(1024)]
        public string SubcategoryName { get; set; } = null!;

        [InverseProperty("IdSubcategoryNavigation")]
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
