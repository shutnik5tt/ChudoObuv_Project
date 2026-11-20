using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ChudoObuv_Project.Models;

public partial class Manufacturer
{
    [Key]
    public int Id { get; set; }

    [StringLength(1024)]
    public string ManufacturerName { get; set; } = null!;

    [InverseProperty("IdManufacturerNavigation")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
