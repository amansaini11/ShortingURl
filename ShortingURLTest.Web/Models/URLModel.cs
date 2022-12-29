namespace ShortingURL.Web.Models
{
    public class URLModel
    {
        public int Id { get; set; }
        public virtual int? AppUserId { get; set; }
        public string TinyUrl { get; set; }
        public string Orignalurl { get; set; }
        public bool IsDeleted { get; set; } = false;
        public List<URLModel> uRLModels { get; set; }
    }
}
