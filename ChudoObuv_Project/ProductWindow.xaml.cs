using System.Globalization;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;

namespace ChudoObuv_Project
{
    public class SizeOption
    {
        public int StockItemId { get; set; }
        public string SizeText { get; set; } = "";
        public int Available { get; set; }

        public string Display
        {
            get { return $"{SizeText} (в наличии: {Available})"; }
        }
    }

    public partial class ProductWindow : Window
    {
        private string _productText = "";
        private int _price;

        public ProductWindow(int productId)
        {
            InitializeComponent();
            LoadProduct(productId);
        }

        private void LoadProduct(int productId)
        {
            try
            {
                using var db = Db.Create();

                var product = db.Products
                    .Include(p => p.IdCategoryNavigation)
                    .Include(p => p.IdManufacturerNavigation)
                    .Include(p => p.StockItems).ThenInclude(s => s.IdSizeNavigation)
                    .First(p => p.Id == productId);

                var orderedLastMonth = PriceCalculator.GetProductIdsOrderedLastMonth(db, DateTime.Today);
                _price = PriceCalculator.GetPriceWithDiscount(product.Price, orderedLastMonth.Contains(product.Id));
                _productText = $"{product.ProductName} ({product.IdManufacturerNavigation.ManufacturerName})";

                ProductImage.Source = ImageLoader.Load(product.ImagePath);
                TitleText.Text = $"{product.IdManufacturerNavigation.ManufacturerName} | {product.ProductName}";
                CategoryText.Text = $"Категория: {product.IdCategoryNavigation.CategoryName}";
                PriceText.Text = $"{_price} ₽";
                CompositionText.Text = $"Состав: {product.Composition}";
                DescriptionText.Text = $"Описание: {product.Description}";

                if (_price != product.Price)
                {
                    OldPriceText.Text = $"{product.Price} ₽";
                }

                var allSizes = product.StockItems
                    .OrderBy(s => SizeToNumber(s.IdSizeNavigation.SizeValue))
                    .Select(s => s.IdSizeNavigation.SizeValue);
                SizeRangeText.Text = "Размерный ряд: " + string.Join(", ", allSizes);

                var options = product.StockItems
                    .OrderBy(s => SizeToNumber(s.IdSizeNavigation.SizeValue))
                    .Select(s => new SizeOption
                    {
                        StockItemId = s.Id,
                        SizeText = s.IdSizeNavigation.SizeValue,
                        Available = s.Quantity - OrderDraft.GetQuantity(s.Id)
                    })
                    .Where(o => o.Available > 0)
                    .ToList();

                SizeBox.ItemsSource = options;

                if (options.Count > 0)
                {
                    SizeBox.SelectedIndex = 0;
                }
                else
                {
                    AddButton.IsEnabled = false;
                    Messages.Info("Для этого товара нет размеров, доступных для заказа.");
                }
            }
            catch (Exception ex)
            {
                Messages.Error("Не удалось загрузить данные товара.\n\n" + ex.Message);
                Close();
            }
        }

        private static double SizeToNumber(string size)
        {
            double.TryParse(size.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out double number);
            return number;
        }

        private void QuantityBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (SizeBox.SelectedItem is not SizeOption size)
            {
                Messages.Warning("Выберите размер обуви в выпадающем списке.");
                return;
            }

            if (!int.TryParse(QuantityBox.Text, out int quantity) || quantity < 1)
            {
                Messages.Warning("Введите количество пар — целое число не меньше 1.");
                return;
            }

            if (quantity > size.Available)
            {
                Messages.Warning($"Для размера {size.SizeText} доступно только {size.Available} пар. Уменьшите количество.");
                return;
            }

            OrderDraft.Add(new OrderLine
            {
                StockItemId = size.StockItemId,
                ProductText = _productText,
                SizeText = size.SizeText,
                Quantity = quantity,
                PricePerUnit = _price
            });

            Messages.Info("Товар добавлен в заказ. Оформить заказ можно кнопкой «Мой заказ».");
            Close();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
