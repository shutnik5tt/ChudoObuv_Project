using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ChudoObuv_Project.Models;

public partial class Category
{
    [Key]
    public int Id { get; set; }

    [StringLength(1024)]
    public string CategoryName { get; set; } = null!;

    [InverseProperty("IdCategoryNavigation")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
