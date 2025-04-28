namespace AspireAppTest.ApiService.Data
{
    public class Sale
    {
        public Guid Id { get; set; }
        public AspireAppTest.ApiService.Data.CoffeeType CoffeeType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
