namespace AppCalisto.Data.Entities
{
    public class BudgetProduct
    {
        public Budget Budget { get; set; }

        public int BudgetId { get; set; }

        public Product Product { get; set; }

        public int ProductId { get; set; }
    }
}
