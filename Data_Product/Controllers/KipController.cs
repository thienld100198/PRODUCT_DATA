using Data_Product.Models;
using Data_Product.Repositorys;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Data_Product.Controllers
{
    public class KipController : Controller
    {
        private readonly DataContext _context;

        public KipController(DataContext _context)
        {
            this._context = _context;
        }
        public async Task<IActionResult> Index(string search, int page = 1)
        {
            var res = await (from a in _context.Tbl_Kip
                             select new Tbl_Kip
                             {
                                 ID_Kip = a.ID_Kip,
                                 TenKip = a.TenKip,
                                 TenCa = a.TenCa,
                                 TuNgay = (DateTime?)a.TuNgay ?? default,
                                 DenNgay = (DateTime?)a.DenNgay ?? default
                             }).ToListAsync();
            if (search != null)
            {
                res = res.Where(x => x.TenKip.Contains(search) || x.TenCa.Contains(search)).ToList();

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

            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tbl_Kip _DO)
        {
            DateTime NgayTao = DateTime.Now;
            try
            {
                var result = _context.Database.ExecuteSqlRaw("EXEC Tbl_Kip_insert {0},{1},{2},{3}", _DO.TuNgay,_DO.DenNgay, _DO.TenKip, _DO.TenCa);

                TempData["msgSuccess"] = "<script>alert('Thêm mới thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Thêm mới thất bại');</script>";
            }

            return RedirectToAction("Index", "Kip");
        }
        public async Task<IActionResult> Edit(int? id, int? page)
        {
            if (id == null)
            {
                TempData["msgError"] = "<script>alert('Chỉnh sửa thất bại');</script>";

                return RedirectToAction("Index", "Kip");
            }

            var res = await (from a in _context.Tbl_Kip.Where(x => x.ID_Kip == id)
                             select new Tbl_Kip
                             {
                                 ID_Kip = a.ID_Kip,
                                 TenKip = a.TenKip,
                                 TenCa = a.TenCa,
                                 TuNgay = (DateTime?)a.TuNgay ?? default,
                                 DenNgay = (DateTime?)a.DenNgay ?? default
                             }).ToListAsync();

            Tbl_Kip DO = new Tbl_Kip();
            if (res.Count > 0)
            {
                foreach (var a in res)
                {
                    DO.ID_Kip = a.ID_Kip;
                    DO.TenKip = a.TenKip;
                    DO.TenCa = a.TenCa;
                    DO.TuNgay = (DateTime?)a.TuNgay ?? default;
                    DO.DenNgay = (DateTime?)a.DenNgay ?? default;
                }

                DateTime TuNgay = (DateTime)DO.TuNgay;
                ViewBag.TuNgay = TuNgay.ToString("yyyy-MM-dd");

                DateTime DenNgay = (DateTime)DO.DenNgay;
                ViewBag.DenNgay = DenNgay.ToString("yyyy-MM-dd");
            }
            else
            {
                return NotFound();
            }



            return PartialView(DO);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Tbl_Kip _DO)
        {
            try
            {
                var result = _context.Database.ExecuteSqlRaw("EXEC Tbl_Kip_update {0},{1},{2},{3},{4}", id, _DO.TuNgay, _DO.DenNgay, _DO.TenKip,_DO.TenCa);

                TempData["msgSuccess"] = "<script>alert('Chỉnh sửa thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Chính sửa thất bại');</script>";
            }
            return RedirectToAction("Index", "Kip");
        }
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = _context.Database.ExecuteSqlRaw("EXEC Tbl_Kip_delete {0}", id);

                TempData["msgSuccess"] = "<script>alert('Xóa dữ liệu thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Xóa dữ liệu thất bại');</script>";
            }


            return RedirectToAction("Index", "Kip");
        }
    }
}
