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
    public class NguyenLieuController : Controller
    {
        public ActionResult DanhSach()
        {
            var nLieuSrv = IoC.Resolve<INguyenLieuService>();
            var list = nLieuSrv.GetAll();
            return View(list);
        }

        public ActionResult TaoMoi()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TaoMoi(NguyenLieu model)
        {
            try
            {
                var nLieuSrv = IoC.Resolve<INguyenLieuService>();
                //if (ModelState.IsValid)
                // {
                model.Ma = model.Ma.ToUpper();
                var nLieuExist = nLieuSrv.GetNguyenLieuByMa(model.Ma);
                if (nLieuExist != null)
                {
                    TempData["ThongBao"] = "Mã nguyên liệu đã tồn tại!";
                    TempData["LoaiThongBao"] = "danger";
                    return View(model);
                }

                nLieuSrv.Save(model);
                nLieuSrv.CommitChanges();
                TempData["ThongBao"] = "Thêm nguyên liệu thành công!";
                TempData["LoaiThongBao"] = "success";
                return RedirectToAction("DanhSach");
                //}
                // TempData["ThongBao"] = "Dữ liệu lỗi!";
                //TempData["LoaiThongBao"] = "danger";
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
            var _dichVuService = IoC.Resolve<INguyenLieuService>();
            var obj = _dichVuService.Getbykey(id);
            return View(obj);
        }

        [HttpPost]
        public ActionResult ChinhSua(NguyenLieu model)
        {
            try
            {
                var _dichVuService = IoC.Resolve<INguyenLieuService>();
                if (ModelState.IsValid)
                {
                    _dichVuService.Save(model);
                    _dichVuService.CommitChanges();
                    TempData["ThongBao"] = "Sửa nguyên liệu thành công!";
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
                var dichVuNLSrv = IoC.Resolve<IDichVuNguyenLieuService>();
                var nguyenLieu = dichVuNLSrv.GetByNguyenLieuId(id);
                if (nguyenLieu != null)
                {
                    TempData["ThongBao"] = "Nguyên liệu đã tồn tại trong dịch vụ nên không thể xóa!";
                    TempData["LoaiThongBao"] = "danger";
                    return RedirectToAction("DanhSach");
                }
                var nLieuSrv = IoC.Resolve<INguyenLieuService>();
                var obj = nLieuSrv.Getbykey(id);
                nLieuSrv.Delete(obj);
                nLieuSrv.CommitChanges();
                TempData["ThongBao"] = "Xóa nguyên liệu thành công!";
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