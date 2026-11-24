using ChudoObuv_Project.Models;

namespace ChudoObuv_Project
{
    public static class PriceCalculator
    {
        private const decimal DiscountRate = 0.25m;

        public static int GetPriceWithDiscount(int price, bool hasOrdersLastMonth)
        {
            if (hasOrdersLastMonth)
            {
                return price;
            }

            decimal discounted = price * (1 - DiscountRate);
            return (int)Math.Round(discounted, MidpointRounding.AwayFromZero);
        }

        public static HashSet<int> GetProductIdsOrderedLastMonth(AppDbContext db, DateTime today)
        {
            var thisMonthStart = new DateTime(today.Year, today.Month, 1);
            var lastMonthStart = thisMonthStart.AddMonths(-1);

            var ids = db.Orders
                .Where(o => o.OrderDate >= lastMonthStart && o.OrderDate < thisMonthStart)
                .Select(o => o.IdStockItemNavigation.IdProduct)
                .Distinct()
                .ToList();

            return ids.ToHashSet();
        }
    }
}
