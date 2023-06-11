using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models {
    public class Article {
        [Key]
        public int Id {set; get;}
        [StringLength(250, MinimumLength = 5, ErrorMessage = "{0} phải dài từ {2} đến {1}")]
        [DisplayName("Tiêu đề")]
        [Required]
        public string Title {get; set;} = default!;
        [Required(ErrorMessage = "{0} phải nhập")]
        [DataType(DataType.Date)]
        [DisplayName("Ngày tạo")]
        public DateTime Created {get; set;}
        [Column(TypeName = "ntext")]
        [DisplayName("Nội dung")]
        public string? Content {set; get;}
    }
}