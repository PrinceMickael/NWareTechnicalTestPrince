namespace BlogEngineNwareTechnologies.Models
{
    public class PostModel : CategoryModel
    {
        public int Id { get; set; }
        public string title { get; set; }
        public CategoryModel category { get; set; }
        public DateTime publicationDate { get; set; }
        public string content { get; set; }
    }
}
