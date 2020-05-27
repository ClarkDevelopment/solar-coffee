namespace SolarCoffee.Web.ViewModels
{
    public class SalesOrderItemModel
    {
        /// <summary>
        /// View Model for SalesOrderItems
        /// </summary>
        public int Id { get; set; }
        public int Quantity { get; set; }
        public ProductModel Product { get; set; }
    }
}