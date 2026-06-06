namespace E_Commerce_Web_API.Models
{
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }=string.Empty;
        public float Price { get; set; }
        public string Description { get; set; }= string.Empty;
        public int CategoryID { get; set; }
        public Category Category { get; set; } = null!;

        public Stock Stock { get; set; }=null!;

        public ICollection<Review> Reviews { get; set; } = new List<Review>();

    }
}
