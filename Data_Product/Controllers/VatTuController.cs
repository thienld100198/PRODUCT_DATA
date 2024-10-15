﻿using Data_Product.Models;
using Data_Product.Repositorys;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Xml.Linq;

namespace Data_Product.Controllers
{
    public class VatTuController : Controller
    {
        private readonly DataContext _context;

        public VatTuController(DataContext _context)
        {
            this._context = _context;
        }
        public async Task<IActionResult> Index(string search, int page = 1)
        {
            var res = await (from a in _context.Tbl_VatTu
                             join vt in _context.Tbl_NhomVatTu on a.ID_NhomVatTu equals vt.ID_NhomVatTu
                             select new Tbl_VatTu
                             {
                                 ID_VatTu = a.ID_VatTu,
                                 TenVatTu = a.TenVatTu,
                                 MaVatTu_Sap = a.MaVatTu_Sap??default,
                                 TenVatTu_Sap = a.TenVatTu_Sap,
                                 DonViTinh = a.DonViTinh,
                                 ID_NhomVatTu = (int)a.ID_NhomVatTu,
                                 TenNhomVatTu = vt.TenNhomVatTu,
                                 PhongBan = a.PhongBan,
                                 ID_TrangThai = (int?)a.ID_TrangThai ?? default
                             }).ToListAsync();
            if (search != null)
            {
                res = res.Where(x => x.TenVatTu.Contains(search)|| x.TenVatTu_Sap.Contains(search)).ToList();

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
        public async Task<IActionResult> Create()
        {
            List<Tbl_NhomVatTu> vt = _context.Tbl_NhomVatTu.ToList();
            ViewBag.VTList = new SelectList(vt, "ID_NhomVatTu", "TenNhomVatTu");

            //List<Tbl_PhongBan> pb = _context.Tbl_PhongBan.ToList();
            //ViewBag.PBList = new SelectList(pb, "ID_PhongBan", "TenPhongBan");


            List<Tbl_PhongBan> pb = _context.Tbl_PhongBan.ToList();
            ViewBag.ID_PhongBan = new SelectList(pb, "ID_PhongBan", "TenPhongBan");

            //List<Tbl_PhongBan> test = _context.Tbl_PhongBan.ToList();
            //ViewBag.Options = new SelectList(test, "ID_PhongBan", "TenPhongBan");

            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tbl_VatTu _DO)
        {
            DateTime NgayTao = DateTime.Now;
            try
            {
                var Name = new List<string>();
                if (_DO.ID_PhongBan != null)
                {
                    foreach (var bp in _DO.ID_PhongBan)
                    {
                        Name.Add(bp);
                    }
                    var ListGate = string.Join(",", Name);

                    var result = _context.Database.ExecuteSqlRaw("EXEC Tbl_VatTu_insert {0},{1},{2},{3},{4},{5},{6}",
                    _DO.TenVatTu, _DO.MaVatTu_Sap, _DO.TenVatTu_Sap, _DO.DonViTinh, _DO.ID_NhomVatTu, ListGate, 1);
                }
                else
                {
                    TempData["msgSuccess"] = "<script>alert('Vui lòng chọn BP/NM');</script>";
                    return RedirectToAction("Index", "VatTu");
                }    

              

                TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Thêm mới thất bại');</script>";
            }

            return RedirectToAction("Index", "VatTu");
        }
        public async Task<IActionResult> Edit(int? id, int? page)
        {
            if (id == null)
            {
                TempData["msgError"] = "<script>alert('Chỉnh sửa thất bại');</script>";

                return RedirectToAction("Index", "VatTu");
            }

            var res = await (from a in _context.Tbl_VatTu.Where(x=>x.ID_VatTu == id)
                             join vt in _context.Tbl_NhomVatTu on a.ID_NhomVatTu equals vt.ID_NhomVatTu
                             select new Tbl_VatTu
                             {
                                 ID_VatTu = a.ID_VatTu,
                                 TenVatTu = a.TenVatTu,
                                 MaVatTu_Sap = a.MaVatTu_Sap??default,
                                 TenVatTu_Sap = a.TenVatTu_Sap,
                                 DonViTinh = a.DonViTinh,
                                 ID_NhomVatTu = (int)a.ID_NhomVatTu,
                                 TenNhomVatTu = vt.TenNhomVatTu,
                                 PhongBan = a.PhongBan,
                                 ID_TrangThai = (int?)a.ID_TrangThai ?? default
                             }).ToListAsync();

            Tbl_VatTu DO = new Tbl_VatTu();
            if (res.Count > 0)
            {
                foreach (var a in res)
                {
                    DO.ID_VatTu = a.ID_VatTu;
                    DO.TenVatTu = a.TenVatTu;
                    DO.MaVatTu_Sap = a.MaVatTu_Sap?? default;
                    DO.TenVatTu_Sap = a.TenVatTu_Sap;
                    DO.DonViTinh = a.DonViTinh;
                    DO.ID_NhomVatTu = (int)a.ID_NhomVatTu;
                    DO.PhongBan = a.PhongBan;
                    DO.ID_TrangThai = (int?)a.ID_TrangThai ?? default;
                }
                List<Tbl_NhomVatTu> vt = _context.Tbl_NhomVatTu.ToList();
                ViewBag.VTList = new SelectList(vt, "ID_NhomVatTu", "TenNhomVatTu", DO.ID_NhomVatTu);

                List<Tbl_PhongBan> pb = _context.Tbl_PhongBan.ToList();
                ViewBag.ID_PhongBan = new SelectList(pb, "ID_PhongBan", "TenPhongBan");
                ViewBag.ID_VatTu = id;
            }
            else
            {
                return NotFound();
            }



            return PartialView(DO);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Tbl_VatTu _DO)
        {
            try
            {
                if (_DO.ID_PhongBan != null)
                {
                    var Name = new List<string>();
                    foreach (var bp in _DO.ID_PhongBan)
                    {
                        Name.Add(bp);
                    }
                    var ListGate = string.Join(",", Name);
                    var result = _context.Database.ExecuteSqlRaw("EXEC Tbl_VatTu_update {0},{1},{2},{3},{4},{5},{6}", id,
                             _DO.TenVatTu, _DO.MaVatTu_Sap, _DO.TenVatTu_Sap, _DO.DonViTinh, _DO.ID_NhomVatTu, ListGate);
                }
                else
                {
                    var DO = _context.Tbl_VatTu.Where(x => x.ID_VatTu == _DO.ID_VatTu).FirstOrDefault();

                    var result = _context.Database.ExecuteSqlRaw("EXEC Tbl_VatTu_update {0},{1},{2},{3},{4},{5},{6}", id,
                        _DO.TenVatTu, _DO.MaVatTu_Sap, _DO.TenVatTu_Sap, _DO.DonViTinh, _DO.ID_NhomVatTu, DO.PhongBan);

                }
                TempData["msgSuccess"] = "<script>alert('Chỉnh sửa thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Chính sửa thất bại');</script>";
            }



            return RedirectToAction("Index", "VatTu");
        }
        public async Task<IActionResult> Lock(int id)
        {
            try
            {

                var result = _context.Database.ExecuteSqlRaw("EXEC Tbl_VatTu_lock {0},{1}", id, 0);

                TempData["msgSuccess"] = "<script>alert('Khóa thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Khóa dữ liệu thất bại');</script>";
            }


            return RedirectToAction("Index", "VatTu");
        }
        public async Task<IActionResult> Unlock(int id)
        {
            try
            {

                var result = _context.Database.ExecuteSqlRaw("EXEC Tbl_VatTu_lock {0},{1}", id, 1);

                TempData["msgSuccess"] = "<script>alert('Mở khóa thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Mở khóa dữ liệu thất bại');</script>";
            }


            return RedirectToAction("Index", "VatTu");
        }
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = _context.Database.ExecuteSqlRaw("EXEC Tbl_VatTu_delete {0}", id);

                TempData["msgSuccess"] = "<script>alert('Xóa dữ liệu thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }


            return RedirectToAction("Index", "VatTu");
        }
        public async Task<IActionResult> ImportExcel()
        {
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return RedirectToAction("Index", "VatTu");
                }


                // Create the Directory if it is not exist
                string webRootPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string dirPath = Path.Combine(webRootPath, "ReceivedReports");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                // MAke sure that only Excel file is used 
                string dataFileName = Path.GetFileName(DateTime.Now.ToString("yyyyMMddHHmm"));

                string extension = Path.GetExtension(dataFileName);

                string[] allowedExtsnions = new string[] { ".xls", ".xlsx" };
                // Make a Copy of the Posted File from the Received HTTP Request
                string saveToPath = Path.Combine(dirPath, dataFileName);

                using (FileStream stream = new FileStream(saveToPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // USe this to handle Encodeing differences in .NET Core
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                // read the excel file
                IExcelDataReader reader = null;
                using (var stream = new FileStream(saveToPath, FileMode.Open))
                {
                    if (extension == ".xlsx")
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    else
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                    DataSet ds = new DataSet();
                    ds = reader.AsDataSet();
                    reader.Close();
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        System.Data.DataTable serviceDetails = ds.Tables[0];

                        for (int i = 1; i < serviceDetails.Rows.Count; i++)
                        {
                            string TenNguyenLieu = serviceDetails.Rows[i][1].ToString().Trim();
                            string TenBP = serviceDetails.Rows[i][2].ToString().Trim();
                            string[] arrList = TenBP.Trim().Split(',');
                            foreach (var ar in arrList)
                            {
                                if (ar != "")
                                {
                                    var check_bp = _context.Tbl_PhongBan.Where(x => x.TenNgan == ar.Trim()).FirstOrDefault();
                                    if (check_bp == null)
                                    {
                                        TempData["msgSuccess"] = "<script>alert('Vui lòng kiểm tra tên BP/NM: " + TenNguyenLieu + "');</script>";
                                        return RedirectToAction("Index", "VatTu");
                                    }
                                }

                            }
         
                            string TenNhom = serviceDetails.Rows[i][3].ToString().Trim();
                            var check_nhom = _context.Tbl_NhomVatTu.Where(x=>x.TenNhomVatTu == TenNhom).FirstOrDefault();
                            if (check_nhom == null)
                            {
                                TempData["msgSuccess"] = "<script>alert('Vui lòng kiểm tra tên nhóm vật tư: " + TenNguyenLieu + "');</script>";
                                return RedirectToAction("Index", "VatTu");
                            }
                            string DonViTinh = serviceDetails.Rows[i][4].ToString().Trim();
                            string MaVatTu_Sap  = serviceDetails.Rows[i][5].ToString().Trim();
                            string TenVatTu_Sap = serviceDetails.Rows[i][6].ToString().Trim();
                            if(MaVatTu_Sap == "")
                            {
                                var result = _context.Database.ExecuteSqlRaw("EXEC Tbl_VatTu_insert {0},{1},{2},{3},{4},{5},{6}",
                                 TenNguyenLieu, null, TenVatTu_Sap, DonViTinh, check_nhom.ID_NhomVatTu, TenBP, 1);
                            }   
                            else
                            {

                                var result = _context.Database.ExecuteSqlRaw("EXEC Tbl_VatTu_insert {0},{1},{2},{3},{4},{5},{6}",
                             TenNguyenLieu, MaVatTu_Sap, TenVatTu_Sap, DonViTinh, check_nhom.ID_NhomVatTu, TenBP, 1);
                            }    
                         
                        }
                    }
                }
                TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Thêm mới thất bại');</script>";
            }

            return RedirectToAction("Index", "VatTu");
        }
    }
}
