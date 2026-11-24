using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;

namespace ChudoObuv_Project
{
    public class ProductCard
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public int PriceWithDiscount { get; set; }

        public string Title { get; set; } = "";
        public string CategoryText { get; set; } = "";
        public string QuantityText { get; set; } = "";
        public string Composition { get; set; } = "";
        public string PriceText { get; set; } = "";
        public string OldPriceText { get; set; } = "";
        public Visibility OldPriceVisibility { get; set; } = Visibility.Collapsed;
        public ImageSource? Image { get; set; }
        public Brush CardBrush { get; set; } = Brushes.White;
    }

    public partial class MainWindow : Window
    {
        private static readonly Brush HighlightBrush = CreateHighlightBrush();

        private List<ProductCard> _allCards = new();
        private bool _isReady;

        public MainWindow()
        {
            InitializeComponent();
            ShowUserInfo();
            LoadCategories();
            LoadProducts();
            _isReady = true;
            ApplyFilter();
        }

        private static Brush CreateHighlightBrush()
        {
            var brush = new SolidColorBrush(Color.FromRgb(0xFF, 0x80, 0x80));
            brush.Freeze();
            return brush;
        }

        private void ShowUserInfo()
        {
            UserText.Text = Session.FullName;

            if (!Session.IsAuthorized)
            {
                FilterPanel.Visibility = Visibility.Collapsed;
                OrderButton.Visibility = Visibility.Collapsed;
            }

            if (!Session.IsManagerOrAdmin)
            {
                OrdersButton.Visibility = Visibility.Collapsed;
            }

            UpdateOrderButton();
        }

        private void UpdateOrderButton()
        {
            OrderButton.Content = $"Мой заказ ({OrderDraft.Lines.Count})";
        }

        private void LoadCategories()
        {
            try
            {
                using var db = Db.Create();
                var categories = db.Categories
                    .OrderBy(c => c.CategoryName)
                    .Select(c => c.CategoryName)
                    .ToList();

                categories.Insert(0, "Все категории");
                CategoryBox.ItemsSource = categories;
                CategoryBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Messages.Error("Не удалось загрузить категории товаров.\n\n" + ex.Message);
            }
        }

        private void LoadProducts()
        {
            try
            {
                using var db = Db.Create();

                var products = db.Products
                    .Include(p => p.IdCategoryNavigation)
                    .Include(p => p.IdSubcategoryNavigation)
                    .Include(p => p.IdManufacturerNavigation)
                    .Include(p => p.StockItems)
                    .ToList();

                var orderedLastMonth = PriceCalculator.GetProductIdsOrderedLastMonth(db, DateTime.Today);

                _allCards = products.Select(p => CreateCard(p, orderedLastMonth)).ToList();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                Messages.Error("Не удалось загрузить список товаров. Проверьте подключение к базе данных.\n\n" + ex.Message);
            }
        }

        private static ProductCard CreateCard(Models.Product product, HashSet<int> orderedLastMonth)
        {
            int total = product.StockItems.Sum(s => s.Quantity);
            int newPrice = PriceCalculator.GetPriceWithDiscount(product.Price, orderedLastMonth.Contains(product.Id));
            bool hasDiscount = newPrice != product.Price;

            return new ProductCard
            {
                Id = product.Id,
                Name = product.ProductName,
                Description = product.Description ?? "",
                CategoryName = product.IdCategoryNavigation.CategoryName,
                PriceWithDiscount = newPrice,
                Title = $"{product.IdManufacturerNavigation.ManufacturerName} | {product.ProductName}",
                CategoryText = $"Категория: {product.IdCategoryNavigation.CategoryName}",
                QuantityText = total > 5 ? "Количество: много" : "Количество: мало",
                Composition = $"Состав: {product.Composition}",
                PriceText = $"{newPrice} ₽",
                OldPriceText = $"{product.Price} ₽",
                OldPriceVisibility = hasDiscount ? Visibility.Visible : Visibility.Collapsed,
                Image = ImageLoader.Load(product.ImagePath),
                CardBrush = total <= 3 ? HighlightBrush : Brushes.White
            };
        }

        private void ApplyFilter()
        {
            IEnumerable<ProductCard> list = _allCards;
            string search = SearchBox.Text.Trim().ToLower();

            if (search != "")
            {
                list = list.Where(c => c.Name.ToLower().Contains(search) || c.Description.ToLower().Contains(search));
            }

            if (CategoryBox.SelectedIndex > 0)
            {
                string category = (string)CategoryBox.SelectedItem;
                list = list.Where(c => c.CategoryName == category);
            }

            if (SortBox.SelectedIndex == 1)
            {
                list = list.OrderBy(c => c.PriceWithDiscount);
            }
            else if (SortBox.SelectedIndex == 2)
            {
                list = list.OrderByDescending(c => c.PriceWithDiscount);
            }

            ProductsList.ItemsSource = list.ToList();
        }

        private void Filter_Changed(object sender, RoutedEventArgs e)
        {
            if (_isReady)
            {
                ApplyFilter();
            }
        }

        private void Card_Click(object sender, MouseButtonEventArgs e)
        {
            if (!Session.IsAuthorized)
            {
                Messages.Info("Оформлять заказ могут только авторизованные пользователи. Нажмите «Выйти» и войдите по логину.");
                return;
            }

            var card = (ProductCard)((FrameworkElement)sender).DataContext;
            new ProductWindow(card.Id) { Owner = this }.ShowDialog();
            UpdateOrderButton();
        }

        private void Order_Click(object sender, RoutedEventArgs e)
        {
            new OrderWindow { Owner = this }.ShowDialog();
            LoadProducts();
            UpdateOrderButton();
        }

        private void Orders_Click(object sender, RoutedEventArgs e)
        {
            new OrdersWindow { Owner = this }.ShowDialog();
            LoadProducts();
            UpdateOrderButton();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Session.CurrentUser = null;
            OrderDraft.Lines.Clear();
            new LoginWindow().Show();
            Close();
        }
    }
}
