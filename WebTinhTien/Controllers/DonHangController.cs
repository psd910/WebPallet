using CoreWebTinhTien.Domain;
using CoreWebTinhTien.Ioc;
using CoreWebTinhTien.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebTinhTien.Controllers
{
    public class DonHangController : Controller
    {
        public ActionResult DanhSach()
        {
            var _dichVuService = IoC.Resolve<IDonHangService>();
            var list = _dichVuService.GetAll();
            return View(list);
        }

        public ActionResult TaoMoi()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TaoMoi(DonHang model)
        {
            var _dichVuService = IoC.Resolve<IDonHangService>();
            if (ModelState.IsValid)
            {
                _dichVuService.Save(model);
                return RedirectToAction("DanhSach");
            }
            return View(model);
        }

        public ActionResult ChinhSua(int id)
        {
            var _dichVuService = IoC.Resolve<IDonHangService>();
            var obj = _dichVuService.Getbykey(id);
            return View(obj);
        }

        [HttpPost]
        public ActionResult ChinhSua(DonHang model)
        {
            var _dichVuService = IoC.Resolve<IDonHangService>();
            if (ModelState.IsValid)
            {
                _dichVuService.Save(model);
                _dichVuService.CommitChanges();
                return RedirectToAction("DanhSach");
            }
            return View(model);
        }

        public ActionResult Xoa(int id)
        {
            var _dichVuService = IoC.Resolve<IDonHangService>();
            var obj = _dichVuService.Getbykey(id);
            _dichVuService.Delete(obj);
            _dichVuService.CommitChanges();
            return RedirectToAction("DanhSach");
        }
    }
}