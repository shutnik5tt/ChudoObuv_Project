using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ChudoObuv_Project.Models;

public partial class Product
{
    [Key]
    public int Id { get; set; }

    [Column("Id_Category")]
    public int IdCategory { get; set; }

    [Column("Id_Subcategory")]
    public int IdSubcategory { get; set; }

    [StringLength(1024)]
    public string ProductName { get; set; } = null!;

    [Column("Id_Manufacturer")]
    public int IdManufacturer { get; set; }

    [StringLength(1024)]
    public string? Description { get; set; }

    [StringLength(1024)]
    public string? Composition { get; set; }

    public int Price { get; set; }

    [ForeignKey("IdCategory")]
    [InverseProperty("Products")]
    public virtual Category IdCategoryNavigation { get; set; } = null!;

    [ForeignKey("IdManufacturer")]
    [InverseProperty("Products")]
    public virtual Manufacturer IdManufacturerNavigation { get; set; } = null!;

    [ForeignKey("IdSubcategory")]
    [InverseProperty("Products")]
    public virtual Subcategory IdSubcategoryNavigation { get; set; } = null!;

    [InverseProperty("IdProductNavigation")]
    public virtual ICollection<StockItem> StockItems { get; set; } = new List<StockItem>();
}
