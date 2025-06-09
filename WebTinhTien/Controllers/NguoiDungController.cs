using CoreWebTinhTien.Domain;
using CoreWebTinhTien.Ioc;
using CoreWebTinhTien.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTinhTien.Authorize;

namespace WebTinhTien.Controllers
{
    [RBACAuthorize(TenNguoiDung = "admin")]
    public class NguoiDungController : Controller
    {
        public ActionResult DanhSach()
        {
            var nguoiDungSrv = IoC.Resolve<INguoiDungService>();
            var ds = nguoiDungSrv.GetAll();
            return View(ds);
        }
        public ActionResult TaoMoi()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TaoMoi(NguoiDung model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var nguoiDungSrv = IoC.Resolve<INguoiDungService>();
                    nguoiDungSrv.CreateNew(model);
                    nguoiDungSrv.CommitChanges();
                    TempData["ThongBao"] = "Tạo tài khoản thành công!";
                    TempData["LoaiThongBao"] = "success";
                    return RedirectToAction("DanhSach");
                }
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "Có lỗi: " + ex.Message;
                TempData["LoaiThongBao"] = "danger";
            }
            return View(model);
        }

        public ActionResult ChinhSua(int id)
        {
            try
            {

                var nguoiDungSrv = IoC.Resolve<INguoiDungService>();
                var nguoiDung = nguoiDungSrv.Getbykey(id);
                if (nguoiDung == null)
                {
                    TempData["ThongBao"] = "Không tìm thấy người dùng!";
                    TempData["LoaiThongBao"] = "danger";
                    return RedirectToAction("DanhSach");
                }
                return View(nguoiDung);
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "Có lỗi: " + ex.Message;
                TempData["LoaiThongBao"] = "danger";
            }
            return RedirectToAction("DanhSach");
        }

        [HttpPost]
        public ActionResult ChinhSua(NguoiDung model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var nguoiDungSrv = IoC.Resolve<INguoiDungService>();
                    nguoiDungSrv.Update(model);
                    nguoiDungSrv.CommitChanges();
                    TempData["ThongBao"] = "Cập nhật người dùng thành công!";
                    TempData["LoaiThongBao"] = "success";
                    return RedirectToAction("DanhSach");
                }

            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "Có lỗi: " + ex.Message;
                TempData["LoaiThongBao"] = "danger";
            }
            return View(model);
        }
        public ActionResult Xoa(int id)
        {
            try
            {

                var nguoiDungSrv = IoC.Resolve<INguoiDungService>();
                var nguoiDung = nguoiDungSrv.Getbykey(id);
                if (nguoiDung == null)
                {
                    TempData["ThongBao"] = "Không tìm thấy người dùng!";
                    TempData["LoaiThongBao"] = "danger";
                    return RedirectToAction("DanhSach");
                }
                if (nguoiDung.TenDangNhap.ToLower() == "admin")
                {
                    TempData["ThongBao"] = "Không được xóa tài khoản admin!";
                    TempData["LoaiThongBao"] = "danger";
                    return RedirectToAction("DanhSach");
                }
                nguoiDungSrv.Delete(nguoiDung);
                nguoiDungSrv.CommitChanges();
                TempData["ThongBao"] = "Xóa người dùng thành công!";
                TempData["LoaiThongBao"] = "success";
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "Có lỗi: " + ex.Message;
                TempData["LoaiThongBao"] = "danger";
            }
            return RedirectToAction("DanhSach");
        }
    }
}