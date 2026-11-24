namespace ChudoObuv_Project
{
    public class OrderLine
    {
        public int StockItemId { get; set; }
        public string ProductText { get; set; } = "";
        public string SizeText { get; set; } = "";
        public int Quantity { get; set; }
        public int PricePerUnit { get; set; }

        public int Sum
        {
            get { return Quantity * PricePerUnit; }
        }
    }

    public static class OrderDraft
    {
        public static List<OrderLine> Lines { get; } = new();

        public static int Total
        {
            get { return Lines.Sum(l => l.Sum); }
        }

        public static void Add(OrderLine line)
        {
            var existing = Lines.FirstOrDefault(l => l.StockItemId == line.StockItemId);

            if (existing == null)
            {
                Lines.Add(line);
            }
            else
            {
                existing.Quantity += line.Quantity;
            }
        }

        public static int GetQuantity(int stockItemId)
        {
            var existing = Lines.FirstOrDefault(l => l.StockItemId == stockItemId);
            return existing?.Quantity ?? 0;
        }
    }
}
