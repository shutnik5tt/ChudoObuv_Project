using System.Windows;
using System.Windows.Input;
using ChudoObuv_Project.Models;

namespace ChudoObuv_Project
{
    public partial class OrderWindow : Window
    {
        public OrderWindow()
        {
            InitializeComponent();
            ShowLines();
        }

        private void ShowLines()
        {
            LinesGrid.ItemsSource = null;
            LinesGrid.ItemsSource = OrderDraft.Lines;
            TotalText.Text = $"Итого: {OrderDraft.Total} ₽";
        }

        private void QuantityBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void DeleteLine_Click(object sender, RoutedEventArgs e)
        {
            if (LinesGrid.SelectedItem is not OrderLine line)
            {
                Messages.Warning("Выберите позицию, которую нужно удалить.");
                return;
            }

            OrderDraft.Lines.Remove(line);
            ShowLines();
        }

        private void ChangeQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (LinesGrid.SelectedItem is not OrderLine line)
            {
                Messages.Warning("Выберите позицию заказа в таблице.");
                return;
            }

            if (!int.TryParse(QuantityBox.Text, out int quantity) || quantity < 1)
            {
                Messages.Warning("Введите количество пар — целое число не меньше 1.");
                return;
            }

            try
            {
                using var db = Db.Create();
                var stockItem = db.StockItems.Find(line.StockItemId);

                if (stockItem == null)
                {
                    Messages.Error("Товарная позиция не найдена в базе данных. Удалите её из заказа.");
                    return;
                }

                if (quantity > stockItem.Quantity)
                {
                    Messages.Warning($"Для товара «{line.ProductText}» размера {line.SizeText} доступно только {stockItem.Quantity} пар. Уменьшите количество.");
                    return;
                }

                line.Quantity = quantity;
                ShowLines();
            }
            catch (Exception ex)
            {
                Messages.Error("Не удалось изменить количество.\n\n" + ex.Message);
            }
        }

        private void CancelOrder_Click(object sender, RoutedEventArgs e)
        {
            if (OrderDraft.Lines.Count == 0)
            {
                Messages.Info("Заказ пуст, отменять нечего.");
                return;
            }

            if (Messages.Confirm("Отменить заказ? Все добавленные позиции будут удалены."))
            {
                OrderDraft.Lines.Clear();
                Close();
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (OrderDraft.Lines.Count == 0)
            {
                Messages.Warning("В заказе нет товаров. Выберите товар в каталоге и добавьте его в заказ.");
                return;
            }

            if (!Messages.Confirm($"Подтвердить заказ на сумму {OrderDraft.Total} ₽?"))
            {
                return;
            }

            try
            {
                using var db = Db.Create();

                foreach (var line in OrderDraft.Lines)
                {
                    var stockItem = db.StockItems.Find(line.StockItemId);

                    if (stockItem == null || stockItem.Quantity < line.Quantity)
                    {
                        int available = stockItem?.Quantity ?? 0;
                        Messages.Warning($"Товар «{line.ProductText}», размер {line.SizeText}: доступно только {available} пар. Удалите позицию и добавьте её заново с другим количеством.");
                        return;
                    }
                }

                DateTime orderDate = DateTime.Now;

                foreach (var line in OrderDraft.Lines)
                {
                    var stockItem = db.StockItems.Find(line.StockItemId)!;
                    stockItem.Quantity -= line.Quantity;

                    db.Orders.Add(new Order
                    {
                        OrderDate = orderDate,
                        IdUser = Session.CurrentUser!.Id,
                        IdStockItem = line.StockItemId,
                        Quantity = line.Quantity,
                        PricePerUnit = line.PricePerUnit
                    });
                }

                db.SaveChanges();
                int total = OrderDraft.Total;
                OrderDraft.Lines.Clear();
                Messages.Info($"Заказ оформлен. Дата заказа: {orderDate:dd.MM.yyyy}. Сумма: {total} ₽.");
                Close();
            }
            catch (Exception ex)
            {
                Messages.Error("Не удалось оформить заказ. Попробуйте ещё раз.\n\n" + ex.Message);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
