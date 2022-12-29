using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShortingURLTest.Web.Entity
{
    [Table("URLMappings")]
    public class URLMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public virtual int? AppUserId { get; set; }
        public string TinyUrl { get; set; }
        public string Orignalurl { get; set; }
        public AppUser? AppUser { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

    }
}
