using System.ComponentModel.DataAnnotations;
namespace Data_Product.Models
{
    public class Tbl_Kip
    {
        [Key]
        public int ID_Kip { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string? TenKip { get; set; }
        public string? TenCa { get; set; }
    }
}
