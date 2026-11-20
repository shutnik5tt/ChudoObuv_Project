using System.Windows;
using System.Windows.Media;
using ChudoObuv_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace ChudoObuv_Project
{
    public class ProductCard
    {
        public string Title { get; set; } = "";
        public string CategoryText { get; set; } = "";
        public string QuantityText { get; set; } = "";
        public string Composition { get; set; } = "";
        public string PriceText { get; set; } = "";
        public ImageSource? Image { get; set; }
        public Brush CardBrush { get; set; } = Brushes.White;
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                using var db = new AppDbContext();

                var products = db.Products
                    .Include(p => p.IdCategoryNavigation)
                    .Include(p => p.IdSubcategoryNavigation)
                    .Include(p => p.IdManufacturerNavigation)
                    .Include(p => p.StockItems)
                    .ToList();

                ProductsList.ItemsSource = products.Select(p =>
                {
                    int total = p.StockItems.Sum(s => s.Quantity);

                    return new ProductCard
                {
                    Title = $"{p.IdManufacturerNavigation.ManufacturerName} | {p.ProductName}",
                    CategoryText = $"Категория: {p.IdCategoryNavigation.CategoryName} / {p.IdSubcategoryNavigation.SubcategoryName}",
                    QuantityText = $"Количество: {total}",
                    Composition = $"Состав: {p.Composition}",
                    PriceText = $"{p.Price} ₽",
                    CardBrush = total <= 3
                        ? (Brush)new BrushConverter().ConvertFromString("#ff8080")!
                        : Brushes.White
                    };
                }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка загрузки товаров");
            }
        }
    }
}
