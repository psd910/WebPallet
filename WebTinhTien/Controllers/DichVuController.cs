using CoreWebTinhTien.Domain;
using CoreWebTinhTien.Ioc;
using CoreWebTinhTien.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTinhTien.Authorize;
using WebTinhTien.Models;

namespace WebTinhTien.Controllers
{
    [RBACAuthorize(TenNguoiDung = "admin")]
    public class DichVuController : Controller
    {
        public ActionResult DanhSach()
        {
            var _dichVuService = IoC.Resolve<IDichVuService>();
            var list = _dichVuService.GetAll();
            return View(list);
        }

        public ActionResult TaoMoi()
        {
            try
            {
                var model = new DichVuModel();
                var nguyenLieuSrv = IoC.Resolve<INguyenLieuService>();
                var nguyenLieus = nguyenLieuSrv.GetAll();
                if (nguyenLieus == null) nguyenLieus = new List<NguyenLieu>();
                model.DanhSachNguyenLieu = nguyenLieus.Select(nl => new DichVuNguyenLieuModel
                {
                    Id = nl.Id,
                    Ma = nl.Ma,
                    Ten = nl.Ten,
                    SoLuong = 0
                }).ToList();
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "Có lỗi: " + ex.Message;
                TempData["LoaiThongBao"] = "danger";
                return RedirectToAction("DanhSach");
            }

        }

        [HttpPost]
        public ActionResult TaoMoi(DichVuModel model)
        {
            var _dichVuService = IoC.Resolve<IDichVuService>();
            if (ModelState.IsValid)
            {
                var dichVu = new DichVu
                {
                    Ten = model.Ten,
                    Ma = model.Ma.ToUpper()
                };
                var dichVuExist = _dichVuService.GetDichVuByMa(dichVu.Ma);
                if (dichVuExist != null)
                {
                    TempData["ThongBao"] = "Mã dịch vụ đã tồn tại!";
                    TempData["LoaiThongBao"] = "danger";
                }
                _dichVuService.BeginTran();
                try
                {
                    _dichVuService.CreateNew(dichVu);
                    // Thêm liên kết nguyên liệu
                    if (model.DanhSachNguyenLieu != null)
                    {
                        var _dichVuNLService = IoC.Resolve<IDichVuNguyenLieuService>();
                        foreach (var nl in model.DanhSachNguyenLieu)
                        {
                            if (nl.SoLuong > 0)
                            {
                                var lienKet = new DichVuNguyenLieu
                                {
                                    DichVuId = dichVu.Id,
                                    NguyenLieuId = nl.Id,
                                    SoLuong = nl.SoLuong,
                                };
                                _dichVuNLService.CreateNew(lienKet);
                            }
                        }
                    }
                    _dichVuService.CommitTran();
                    TempData["ThongBao"] = "Thêm dịch vụ thành công!";
                    TempData["LoaiThongBao"] = "success";
                    return RedirectToAction("DanhSach");
                }
                catch (Exception ex)
                {
                    _dichVuService.RolbackTran();
                    TempData["ThongBao"] = "Có lỗi: " + ex.Message;
                    TempData["LoaiThongBao"] = "danger";
                }
            }
            return View(model);
        }

        public ActionResult ChinhSua(int id)
        {
            try
            {
                var _dichVuService = IoC.Resolve<IDichVuService>();
                var dichVu = _dichVuService.Getbykey(id);
                if (dichVu == null)
                {
                    TempData["ThongBao"] = "Không tìm thấy dịch vụ!";
                    TempData["LoaiThongBao"] = "danger";
                    return RedirectToAction("DanhSach");
                }
                var _dichVuNLService = IoC.Resolve<IDichVuNguyenLieuService>();
                var nguyenLieuDaChon = _dichVuNLService.GetByDichVuId(id);
                var nguyenLieuSrv = IoC.Resolve<INguyenLieuService>();
                var tatCaNguyenLieu = nguyenLieuSrv.GetAll();
                var model = new DichVuModel
                {
                    Id = dichVu.Id,
                    Ma = dichVu.Ma,
                    Ten = dichVu.Ten,
                    DanhSachNguyenLieu = tatCaNguyenLieu.Select(nl =>
                    {
                        var daChon = nguyenLieuDaChon.FirstOrDefault(x => x.NguyenLieuId == nl.Id);
                        return new DichVuNguyenLieuModel
                        {
                            Id = nl.Id,
                            Ma = nl.Ma,
                            Ten = nl.Ten,
                            SoLuong = daChon?.SoLuong ?? 0
                        };
                    }).ToList()
                };
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "Có lỗi: " + ex.Message;
                TempData["LoaiThongBao"] = "danger";
                return RedirectToAction("DanhSach");
            }
        }

        [HttpPost]
        public ActionResult ChinhSua(DichVuModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var _dichVuService = IoC.Resolve<IDichVuService>();
                    var dichVu = _dichVuService.Getbykey(model.Id);
                    if (dichVu == null)
                    {
                        TempData["ThongBao"] = "Không tìm thấy dịch vụ!";
                        TempData["LoaiThongBao"] = "danger";
                        return RedirectToAction("DanhSach");
                    }
                    _dichVuService.BeginTran();
                    try
                    {
                        dichVu.Ten = model.Ten;
                       //dichVu.Ma = model.Ma.ToUpper();
                        _dichVuService.Update(dichVu);
                        // cập nhật các thuộc tính khác nếu có

                        // Xóa liên kết cũ
                        var _dichVuNLService = IoC.Resolve<IDichVuNguyenLieuService>();
                        var nguyenLieues = _dichVuNLService.GetByDichVuId(model.Id);
                        foreach (var item in nguyenLieues)
                        {
                            _dichVuNLService.Delete(item);
                        }

                        // Thêm lại liên kết mới
                        if (model.DanhSachNguyenLieu != null)
                        {
                            foreach (var nl in model.DanhSachNguyenLieu)
                            {
                                if (nl.SoLuong > 0)
                                {
                                    _dichVuNLService.CreateNew(new DichVuNguyenLieu
                                    {
                                        DichVuId = model.Id,
                                        NguyenLieuId = nl.Id,
                                        SoLuong = nl.SoLuong
                                    });
                                }
                            }
                        }
                        _dichVuService.CommitTran();
                        TempData["ThongBao"] = "Cập nhật dịch vụ thành công!";
                        TempData["LoaiThongBao"] = "success";
                        return RedirectToAction("DanhSach");
                    }
                    catch (Exception exx)
                    {
                        TempData["ThongBao"] = "Có lỗi: " + exx.Message;
                        TempData["LoaiThongBao"] = "danger";
                    }

                }
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "Có lỗi: " + ex.Message;
                TempData["LoaiThongBao"] = "danger";
                return View(model);
            }
        }

        public ActionResult Xoa(int id)
        {
            var _dichVuService = IoC.Resolve<IDichVuService>();
            _dichVuService.BeginTran();
            try
            {
                var dichVu = _dichVuService.Getbykey(id);
                if (dichVu == null)
                {
                    TempData["ThongBao"] = "Không tìm thấy dịch vụ!";
                    TempData["LoaiThongBao"] = "danger";
                    return RedirectToAction("DanhSach");
                }

                // Xóa nguyên liệu liên kết
                // Xóa liên kết cũ
                var _dichVuNLService = IoC.Resolve<IDichVuNguyenLieuService>();
                var nguyenLieues = _dichVuNLService.GetByDichVuId(id);
                foreach (var item in nguyenLieues)
                {
                    _dichVuNLService.Delete(item);
                }
                _dichVuService.Delete(dichVu);
                _dichVuService.CommitTran();
                TempData["ThongBao"] = "Xóa dịch vụ thành công!";
                TempData["LoaiThongBao"] = "success";
            }
            catch (Exception ex)
            {
                _dichVuService.RolbackTran();
                TempData["ThongBao"] = "Có lỗi: " + ex.Message;
                TempData["LoaiThongBao"] = "danger";
            }
            return RedirectToAction("DanhSach");
        }
    }
}