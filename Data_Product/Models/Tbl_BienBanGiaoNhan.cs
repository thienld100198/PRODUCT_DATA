﻿using System.ComponentModel.DataAnnotations;
namespace Data_Product.Models
{
    public class Tbl_BienBanGiaoNhan
    {
        [Key]
        public int ID_BBGN { get; set; }
        public int ID_NhanVien_BG { get; set; }
        public int ID_PhongBan_BG { get; set; }
        public int ID_Xuong_BG { get; set; }
        public int ID_ViTri_BG { get; set; }
        public DateTime ThoiGianXuLyBG { get; set; }
        public int ID_TrangThai_BG { get; set; }


        public int ID_NhanVien_BN { get; set; }
        public int ID_PhongBan_BN { get; set; }
        public int ID_Xuong_BN { get; set; }
        public int ID_ViTri_BN { get; set; }
        public Nullable<DateTime> ThoiGianXuLyBN { get; set; }
        public int ID_TrangThai_BN { get; set; }


        public string SoPhieu { get; set; }
        public int ID_TrangThai_BBGN { get; set; }
        public int ID_QuyTrinh { get; set; }

    }
}
