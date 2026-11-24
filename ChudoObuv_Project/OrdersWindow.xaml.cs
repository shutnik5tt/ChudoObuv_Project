using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace ChudoObuv_Project
{
    public class OrderItemView
    {
        public int OrderId { get; set; }
        public string ProductText { get; set; } = "";
        public string SizeText { get; set; } = "";
        public int Quantity { get; set; }
        public int PricePerUnit { get; set; }

        public int Sum
        {
            get { return Quantity * PricePerUnit; }
        }
    }

    public class OrderSummary
    {
        public int IdUser { get; set; }
        public DateTime OrderDate { get; set; }
        public string ClientName { get; set; } = "";
        public List<OrderItemView> Items { get; set; } = new();
    }

    public partial class OrdersWindow : Window
    {
        private List<OrderSummary> _orders = new();

        public OrdersWindow()
        {
            InitializeComponent();

            if (!Session.IsAdmin)
            {
                AdminPanel.Visibility = Visibility.Collapsed;
            }

            DateBox.DisplayDateEnd = DateTime.Today;
            LoadOrders(null);
        }

        private void LoadOrders(OrderSummary? selected)
        {
            try
            {
                using var db = Db.Create();

                var rows = db.Orders
                    .Include(o => o.IdUserNavigation)
                    .Include(o => o.IdStockItemNavigation).ThenInclude(s => s.IdSizeNavigation)
                    .Include(o => o.IdStockItemNavigation).ThenInclude(s => s.IdProductNavigation).ThenInclude(p => p.IdManufacturerNavigation)
                    .ToList();

                _orders = rows
                    .GroupBy(o => new { o.IdUser, o.OrderDate })
                    .Select(g => new OrderSummary
                    {
                        IdUser = g.Key.IdUser,
                        OrderDate = g.Key.OrderDate,
                        ClientName = $"{g.First().IdUserNavigation.LastName} {g.First().IdUserNavigation.FirstName} {g.First().IdUserNavigation.MiddleName}".Trim(),
                        Items = g.Select(o => new OrderItemView
                        {
                            OrderId = o.Id,
                            ProductText = $"{o.IdStockItemNavigation.IdProductNavigation.ProductName} ({o.IdStockItemNavigation.IdProductNavigation.IdManufacturerNavigation.ManufacturerName})",
                            SizeText = o.IdStockItemNavigation.IdSizeNavigation.SizeValue,
                            Quantity = o.Quantity,
                            PricePerUnit = o.PricePerUnit
                        }).ToList()
                    })
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();

                OrdersGrid.ItemsSource = _orders;

                var toSelect = _orders.FirstOrDefault(o => selected != null && o.IdUser == selected.IdUser && o.OrderDate == selected.OrderDate);
                OrdersGrid.SelectedItem = toSelect ?? _orders.FirstOrDefault();
                ShowItems();
            }
            catch (Exception ex)
            {
                Messages.Error("Не удалось загрузить список заказов.\n\n" + ex.Message);
            }
        }

        private void OrdersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowItems();
        }

        private void ShowItems()
        {
            if (OrdersGrid.SelectedItem is OrderSummary order)
            {
                ItemsGrid.ItemsSource = order.Items;
                TotalText.Text = $"Итоговая сумма по заказу: {order.Items.Sum(i => i.Sum)} ₽";
                DateTime date = order.OrderDate.Date;
                DateBox.SelectedDate = date <= DateTime.Today ? date : null;
            }
            else
            {
                ItemsGrid.ItemsSource = null;
                TotalText.Text = "";
            }
        }

        private void AddOrder_Click(object sender, RoutedEventArgs e)
        {
            Messages.Info("Чтобы добавить заказ, выберите товары в каталоге: нажмите на товар, добавьте его в заказ и подтвердите заказ кнопкой «Мой заказ».");
            Close();
        }

        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem is not OrderSummary order)
            {
                Messages.Warning("Выберите заказ в списке.");
                return;
            }

            if (Messages.Confirm("Удалить выбранный заказ? Это действие нельзя отменить. Товары вернутся в наличие."))
            {
                DeleteRows(order.Items.Select(i => i.OrderId).ToList());
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsGrid.SelectedItem is not OrderItemView item)
            {
                Messages.Warning("Выберите позицию в составе заказа.");
                return;
            }

            if (Messages.Confirm("Удалить выбранную позицию из заказа? Товар вернётся в наличие."))
            {
                DeleteRows(new List<int> { item.OrderId });
            }
        }

        private void DeleteRows(List<int> orderIds)
        {
            var selected = OrdersGrid.SelectedItem as OrderSummary;

            try
            {
                using var db = Db.Create();

                var rows = db.Orders
                    .Include(o => o.IdStockItemNavigation)
                    .Where(o => orderIds.Contains(o.Id))
                    .ToList();

                foreach (var row in rows)
                {
                    row.IdStockItemNavigation.Quantity += row.Quantity;
                }

                db.Orders.RemoveRange(rows);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Messages.Error("Не удалось удалить данные заказа.\n\n" + ex.Message);
            }

            LoadOrders(selected);
        }

        private void ChangeDate_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem is not OrderSummary order)
            {
                Messages.Warning("Выберите заказ в списке.");
                return;
            }

            if (DateBox.SelectedDate == null)
            {
                Messages.Warning("Выберите новую дату заказа.");
                return;
            }

            DateTime newDate = DateBox.SelectedDate.Value;
            var ids = order.Items.Select(i => i.OrderId).ToList();

            try
            {
                using var db = Db.Create();

                foreach (var row in db.Orders.Where(o => ids.Contains(o.Id)))
                {
                    row.OrderDate = newDate;
                }

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Messages.Error("Не удалось изменить дату заказа.\n\n" + ex.Message);
                return;
            }

            LoadOrders(new OrderSummary { IdUser = order.IdUser, OrderDate = newDate });
            Messages.Info("Дата заказа изменена.");
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
