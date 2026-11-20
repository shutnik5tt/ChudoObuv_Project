using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ChudoObuv_Project.Models;

public partial class Order
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime OrderDate { get; set; }

    [Column("Id_User")]
    public int IdUser { get; set; }

    [Column("Id_StockItem")]
    public int IdStockItem { get; set; }

    public int Quantity { get; set; }

    public int PricePerUnit { get; set; }

    [ForeignKey("IdStockItem")]
    [InverseProperty("Orders")]
    public virtual StockItem IdStockItemNavigation { get; set; } = null!;

    [ForeignKey("IdUser")]
    [InverseProperty("Orders")]
    public virtual User IdUserNavigation { get; set; } = null!;
}
