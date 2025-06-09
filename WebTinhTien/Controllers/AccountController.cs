
using CoreWebTinhTien.IService;
using System.Web.Mvc;
using WebTinhTien.Models;
using CoreWebTinhTien.Ioc;
using System;

namespace WebTinhTien.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            if (Session["NguoiDung"] != null) return RedirectToAction("Index", "Home");
            return View();
        }
        [HttpPost]
        public ActionResult Login(NguoiDungModel model)
        {
            if (Session["NguoiDung"] != null) return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                var nguoiDungSrv = IoC.Resolve<INguoiDungService>();
                var nguoiDung = nguoiDungSrv.LayNguoiDungBangTaiKhoanMatKhau(model.UserName, model.Password);
                // Ví dụ xác thực đơn giản (bạn có thể tích hợp Identity hoặc DB sau)
                if (nguoiDung != null)
                {
                    // Tạo session, cookie, hoặc FormsAuthentication ở đây
                    Session["NguoiDung"] = nguoiDung.TenDangNhap;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                }
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            // Xoá session
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }
        
    }
}