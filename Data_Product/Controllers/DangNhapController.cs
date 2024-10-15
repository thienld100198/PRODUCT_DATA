using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Data_Product.Models;
using Data_Product.Repositorys;
using Data_Product.Common;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Data_Product.Controllers
{
    public class DangNhapController : Controller
    {
        private readonly DataContext _context;

        public DangNhapController(DataContext _context)
        {
            this._context = _context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DangNhapTaiKhoan(Tbl_TaiKhoan u)
        {
            if (u.TenTaiKhoan != "" && u.MatKhau != "" && u.TenTaiKhoan != null && u.MatKhau != null)
            {
                string mk = Common.Encryptor.MD5Hash(u.MatKhau);
                Tbl_TaiKhoan? user = _context.Tbl_TaiKhoan?.Where(x => x.TenTaiKhoan == u.TenTaiKhoan && x.MatKhau == mk)?.FirstOrDefault();
                if (user != null)
                {
                    var identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, user?.TenTaiKhoan)
                    }, CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);

                    var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    TempData["msglg"] = "<script>alert('Tài khoản hoặc mật khẩu không đúng, liên hệ B.CNTT nếu bạn quên mật khẩu')</script>";
                    return RedirectToAction("", "Login");
                }
            }
            else
            {
                TempData["msglg"] = "<script>alert('Vui lòng nhập tài khoản và mật khẩu')</script>";
                return RedirectToAction("", "Login");
            }
        }
        public async Task<IActionResult> DangXuatTaiKhoan()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "DangNhap");

        }

        public async Task<IActionResult> ThongTinTaiKhoan()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThongTinTaiKhoan(int id, Tbl_TaiKhoan _DO)
        {
            try
            {
                //var result = _context.Database.ExecuteSqlRaw("EXEC Tbl_PhongBan_update {0},{1},{2}", id, _DO.TenPhongBan, _DO.TenNgan);

                TempData["msgSuccess"] = "<script>alert('Chỉnh sửa thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Chính sửa thất bại');</script>";
            }



            return RedirectToAction("Index", "PhongBan");
        }

        public async Task<IActionResult> DoiMatKhau()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiMatKhau(int id, Tbl_TaiKhoan _DO)
        {
            try
            {
                //var result = _context.Database.ExecuteSqlRaw("EXEC Tbl_PhongBan_update {0},{1},{2}", id, _DO.TenPhongBan, _DO.TenNgan);

                TempData["msgSuccess"] = "<script>alert('Chỉnh sửa thành công');</script>";
            }
            catch (Exception e)
            {
                TempData["msgError"] = "<script>alert('Chính sửa thất bại');</script>";
            }



            return RedirectToAction("Index", "PhongBan");
        }
    }
}
