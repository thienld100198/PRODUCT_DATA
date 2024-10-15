using Data_Product.Models;
using Data_Product.Repositorys;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.Data.SqlClient;

namespace Data_Product.Controllers
{
    public class BM_11Controller : Controller
    {
        private readonly DataContext _context;

        public BM_11Controller(DataContext _context)
        {
            this._context = _context;
        }
        public async Task<IActionResult> Index(string search, int page = 1)
        {
            var TenTaiKhoan = User.FindFirstValue(ClaimTypes.Name);
            var TaiKhoan = _context.Tbl_TaiKhoan.Where(x => x.TenTaiKhoan == TenTaiKhoan).FirstOrDefault();
            int ID_NhanVien_BG = TaiKhoan.ID_TaiKhoan;
            var res = await (from a in _context.Tbl_BienBanGiaoNhan.Where(x => x.ID_NhanVien_BG == ID_NhanVien_BG)
                             select new Tbl_BienBanGiaoNhan
                             {
                                 ID_BBGN = a.ID_BBGN,
                                 ID_NhanVien_BG = a.ID_NhanVien_BG,
                                 ID_PhongBan_BG = a.ID_PhongBan_BG,
                                 ID_Xuong_BG = a.ID_Xuong_BG,
                                 ID_ViTri_BG = a.ID_ViTri_BG,
                                 ThoiGianXuLyBG = (DateTime)a.ThoiGianXuLyBG,
                                 ID_TrangThai_BG = a.ID_TrangThai_BG,
                                 ID_NhanVien_BN = a.ID_NhanVien_BN,
                                 ID_PhongBan_BN = a.ID_PhongBan_BN,
                                 ID_Xuong_BN = a.ID_Xuong_BN,
                                 ID_ViTri_BN = a.ID_ViTri_BN,
                                 ThoiGianXuLyBN = (DateTime?)a.ThoiGianXuLyBN ?? default,
                                 ID_TrangThai_BN = a.ID_TrangThai_BN,
                                 SoPhieu = a.SoPhieu,
                                 ID_TrangThai_BBGN = a.ID_TrangThai_BBGN,
                                 ID_QuyTrinh = a.ID_QuyTrinh
                             }).ToListAsync();

            if (search != null)
            {
                res = res.Where(x => x.SoPhieu.Contains(search)).ToList();

            }
            const int pageSize = 20;
            if (page < 1)
            {
                page = 1;
            }
            int resCount = res.Count;
            var pager = new Pager(resCount, page, pageSize);
            int recSkip = (page - 1) * pageSize;
            var data = res.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;
            return View(data);
        }
        public async Task<IActionResult> Index_Detai(int id)
        { 
            var res = await (from a in _context.Tbl_ChiTiet_BienBanGiaoNhan.Where(x => x.ID_BBGN == id)
                             join vt in _context.Tbl_VatTu on a.ID_VatTu equals vt.ID_VatTu
                             select new Tbl_ChiTiet_BienBanGiaoNhan
                             {
                                 ID_CT_BBGN = a.ID_CT_BBGN,
                                 ID_VatTu = a.ID_VatTu,
                                 TenVatTu = vt.TenVatTu,
                                 DonViTinh = vt.DonViTinh,
                                 MaLo = a.MaLo,
                                 DoAm_W = (double)a.DoAm_W,
                                 KhoiLuong_BG = (double)a.KhoiLuong_BG,
                                 KL_QuyKho_BG = (double)a.KL_QuyKho_BG,
                                 KhoiLuong_BN = (double)a.KhoiLuong_BN,
                                 KL_QuyKho_BN = (double)a.KL_QuyKho_BN,
                                 GhiChu = a.GhiChu,
                                 ID_BBGN = a.ID_BBGN
                             }).ToListAsync();
            ViewBag.Data = id;
            return View(res);
        }


        public async Task<IActionResult> TaoPhieu()
        {
            var TenTaiKhoan = User.FindFirstValue(ClaimTypes.Name);
            var TaiKhoan = _context.Tbl_TaiKhoan.Where(x => x.TenTaiKhoan == TenTaiKhoan).FirstOrDefault();
            var PhongBan = _context.Tbl_PhongBan.Where(x => x.ID_PhongBan == TaiKhoan.ID_PhongBan).FirstOrDefault();
            string TenBP = PhongBan.TenNgan.ToString();

            List<Tbl_NhomVatTu> vt = _context.Tbl_NhomVatTu.ToList();
            ViewBag.VTList = new SelectList(vt, "ID_NhomVatTu", "TenNhomVatTu");

            List<Tbl_PhongBan> pb = _context.Tbl_PhongBan.ToList();
            ViewBag.ID_PhongBan = new SelectList(pb, "ID_PhongBan", "TenPhongBan");

            var NhanVien = await (from a in _context.Tbl_TaiKhoan
                             select new Tbl_TaiKhoan
                             {
                                 ID_TaiKhoan = a.ID_TaiKhoan,
                                 HoVaTen = a.TenTaiKhoan + " - " + a.HoVaTen
                             }).ToListAsync();

            ViewBag.IDTaiKhoan = new SelectList(NhanVien, "ID_TaiKhoan", "HoVaTen");

            var VatTu = await (from a in _context.Tbl_VatTu.Where(x=>x.PhongBan.Contains(TenBP))
                                  select new Tbl_VatTu
                                  {
                                      ID_VatTu = a.ID_VatTu,
                                      TenVatTu = a.TenVatTu
                                  }).ToListAsync();

            ViewBag.VTList = new SelectList(VatTu, "ID_VatTu", "TenVatTu");

            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoPhieu(Tbl_ChiTiet_BienBanGiaoNhan _DO, IFormCollection formCollection)
        {
            int IDTaiKhoan = 0;
            string XacNhan = "";
            int BBGN_ID = 0;
            List<Tbl_ChiTiet_BienBanGiaoNhan> Tbl_ChiTiet_BienBanGiaoNhan = new List<Tbl_ChiTiet_BienBanGiaoNhan>();
            try
            {

                foreach (var key in formCollection.ToList())
                {
                   

                    if (key.Key != "__RequestVerificationToken")
                    {
                        IDTaiKhoan = Convert.ToInt32(formCollection["IDTaiKhoan"]);
                        XacNhan = formCollection["xacnhan"];
                    }
                    if (key.Key != "__RequestVerificationToken" && key.Key != "IDTaiKhoan" && key.Key != "xacnhan" && key.Key == "ghichu_" + key.Key.Split('_')[1] )
                    {
                        Tbl_ChiTiet_BienBanGiaoNhan.Add(new Tbl_ChiTiet_BienBanGiaoNhan()
                        {
                            ID_VatTu = Convert.ToInt32(formCollection["VatTu_" + key.Key.Split('_')[1]]),
                            MaLo = formCollection["lo_" + key.Key.Split('_')[1]],
                            DoAm_W = double.Parse(formCollection["doam_" + key.Key.Split('_')[1]]),
                            KhoiLuong_BG = double.Parse(formCollection["khoiluongbg_" + key.Key.Split('_')[1]]),
                            KL_QuyKho_BG = double.Parse(formCollection["quykhobg_" + key.Key.Split('_')[1]]),
                            GhiChu = formCollection["ghichu_" + key.Key.Split('_')[1]]
                        });
                    }
                }
                //Inser thông tin phiếu giao nhận
                if(IDTaiKhoan != 0 && XacNhan != "")
                {
                
                    // Thông tin bên giao
                    var MBVN_BG = User.FindFirstValue(ClaimTypes.Name);
                    var ThongTin_BG = _context.Tbl_TaiKhoan.Where(x => x.TenTaiKhoan == MBVN_BG).FirstOrDefault();
                    var ThongTin_BP_BG = _context.Tbl_PhongBan.Where(x => x.ID_PhongBan == ThongTin_BG.ID_PhongBan).FirstOrDefault();

                    // Thông tin bên nhận
                    var ThongTin_BN = _context.Tbl_TaiKhoan.Where(x => x.ID_TaiKhoan == IDTaiKhoan).FirstOrDefault();
                    var ThongTin_BP_BN = _context.Tbl_PhongBan.Where(x => x.ID_PhongBan == ThongTin_BN.ID_PhongBan).FirstOrDefault();

                    DateTime ThoiGianXuLyBG = DateTime.Now;
                    int ID_TrangThai_BG = Convert.ToInt32(XacNhan);
                    string stt = "";
                    var count = _context.Tbl_BienBanGiaoNhan.Where(x => x.ThoiGianXuLyBG == ThoiGianXuLyBG).Count();
                    int len = count.ToString().Length;
                    if (len == 1)
                    {
                        stt = "00" + (count + 1);
                    }   
                    else if(len == 2) 
                    {
                        stt = "0" + (count + 1);
                    }    
                    else if(len == 3)
                    {
                        stt = (count + 1).ToString();
                    }    
                    string SoPhieu = ThongTin_BP_BG.TenNgan + "-" + ThongTin_BP_BN.TenNgan + "-" + "1C" + "-" +
                                        ThoiGianXuLyBG.ToString("dd") + ThoiGianXuLyBG.ToString("MM") + ThoiGianXuLyBG.ToString("yy") + stt;

                    var Output_ID_BBGN = new SqlParameter
                    {
                        ParameterName = "ID_BBGN",
                        SqlDbType = System.Data.SqlDbType.Int,
                        Direction = System.Data.ParameterDirection.Output,
                    };
                    var result = _context.Database.ExecuteSqlRaw("EXEC Tbl_BienBanGiaoNhan_insert {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},@ID_BBGN OUTPUT",
                                                                ThongTin_BG.ID_TaiKhoan, ThongTin_BG.ID_PhongBan, ThongTin_BG.ID_PhanXuong, ThongTin_BG.ID_ChucVu, ThoiGianXuLyBG, ID_TrangThai_BG,
                                                                ThongTin_BN.ID_TaiKhoan, ThongTin_BN.ID_PhongBan, ThongTin_BN.ID_PhanXuong, ThongTin_BN.ID_ChucVu,0, SoPhieu,0,1, Output_ID_BBGN);
                    BBGN_ID = Convert.ToInt32(Output_ID_BBGN.Value);
                }
                foreach (var item in Tbl_ChiTiet_BienBanGiaoNhan)
                {
                    if(BBGN_ID != 0)
                    {
                        var result_Vitri = _context.Database.ExecuteSqlRaw("EXEC Tbl_ChiTiet_BienBanGiaoNhan_insert {0},{1},{2},{3},{4},{5},{6},{7},{8}",
                                                                     item.ID_VatTu, item.MaLo, item.DoAm_W, item.KhoiLuong_BG, item.KL_QuyKho_BG, 0, 0, item.GhiChu, BBGN_ID);
                    }    
                 
                }    


            TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Thêm mới thất bại');</script>";
            }

            return RedirectToAction("TaoPhieu", "BM_11");
        }


        public async Task<IActionResult> SuaPhieu(int? id)
        {
            var TenTaiKhoan = User.FindFirstValue(ClaimTypes.Name);
            var TaiKhoan = _context.Tbl_TaiKhoan.Where(x => x.TenTaiKhoan == TenTaiKhoan).FirstOrDefault();
            var PhongBan = _context.Tbl_PhongBan.Where(x => x.ID_PhongBan == TaiKhoan.ID_PhongBan).FirstOrDefault();
            string TenBP = PhongBan.TenNgan.ToString();

            var ID_BBGN = _context.Tbl_BienBanGiaoNhan.Where(x=>x.ID_BBGN == id).FirstOrDefault();


            List<Tbl_PhongBan> pb = _context.Tbl_PhongBan.ToList();
            ViewBag.ID_PhongBan = new SelectList(pb, "ID_PhongBan", "TenPhongBan", ID_BBGN.ID_PhongBan_BN);

            var NhanVien = await (from a in _context.Tbl_TaiKhoan
                                  select new Tbl_TaiKhoan
                                  {
                                      ID_TaiKhoan = a.ID_TaiKhoan,
                                      HoVaTen = a.TenTaiKhoan + " - " + a.HoVaTen
                                  }).ToListAsync();

            ViewBag.IDTaiKhoan = new SelectList(NhanVien, "ID_TaiKhoan", "HoVaTen", ID_BBGN.ID_NhanVien_BN);

            var VatTu = await (from a in _context.Tbl_VatTu.Where(x => x.PhongBan.Contains(TenBP))
                               select new Tbl_VatTu
                               {
                                   ID_VatTu = a.ID_VatTu,
                                   TenVatTu = a.TenVatTu
                               }).ToListAsync();

            ViewBag.VTList = new SelectList(VatTu, "ID_VatTu", "TenVatTu");
            ViewBag.Data = id;
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaPhieu(int id, Tbl_Kip _DO)
        {
            try
            {
                var result = _context.Database.ExecuteSqlRaw("EXEC Tbl_Kip_update {0},{1},{2},{3},{4}", id, _DO.TuNgay, _DO.DenNgay, _DO.TenKip, _DO.TenCa);

                TempData["msgSuccess"] = "<script>alert('Chỉnh sửa thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Chính sửa thất bại');</script>";
            }
            return RedirectToAction("Index", "Kip");
        }

        public IActionResult BoSungPhieu()
        {
            return View();
        }

        public async Task<IActionResult> ChucVu(int IDTaiKhoan)
        {
            if (IDTaiKhoan == null) IDTaiKhoan = 0;

            var NhanVien = await (from a in _context.Tbl_TaiKhoan.Where(x => x.ID_TaiKhoan == IDTaiKhoan)
                                  join cv in _context.Tbl_ViTri on a.ID_ChucVu equals cv.ID_ViTri
                                  select new Tbl_TaiKhoan
                                  {
                                      ID_TaiKhoan = a.ID_TaiKhoan,
                                      ID_ChucVu = a.ID_ChucVu,
                                      TenChucVu = cv.TenViTri
                                  }).ToListAsync();

            return Json(NhanVien);
        }


        public async Task<IActionResult> PhongBan(int IDTaiKhoan)
        {
            if (IDTaiKhoan == null) IDTaiKhoan = 0;

            var NhanVien = await (from a in _context.Tbl_TaiKhoan.Where(x => x.ID_TaiKhoan == IDTaiKhoan)
                                  join cv in _context.Tbl_PhongBan on a.ID_PhongBan equals cv.ID_PhongBan
                                  select new Tbl_TaiKhoan
                                  {
                                      ID_TaiKhoan = a.ID_TaiKhoan,
                                      ID_PhongBan = a.ID_PhongBan,
                                      TenPhongBan = cv.TenPhongBan
                                  }).ToListAsync();

            return Json(NhanVien);
        }

        public async Task<IActionResult> NguyenLieu(int IDTaiKhoan)
        {
            if (IDTaiKhoan == null) IDTaiKhoan = 0;
            var ID = _context.Tbl_TaiKhoan.Where(x=>x.ID_TaiKhoan == IDTaiKhoan).FirstOrDefault();
            string ID_PhongBan = ID.ID_PhongBan.ToString();

            var NguyenLieu = await (from a in _context.Tbl_VatTu                 
                                  select new Tbl_VatTu
                                  {
                                      ID_VatTu = a.ID_VatTu,
                                      PhongBan = a.PhongBan,
                                      TenVatTu = a.TenVatTu
                                  }).ToListAsync();
            if(ID_PhongBan != "")
            {
                NguyenLieu = NguyenLieu.Where(x => x.PhongBan.Contains(ID_PhongBan)).ToList();
            } 
            return Json(NguyenLieu);
        }
    }
}
