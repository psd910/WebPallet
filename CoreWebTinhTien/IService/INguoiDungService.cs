using CoreWebTinhTien.Domain;
using CoreWebTinhTien.BaseServices;

namespace CoreWebTinhTien.IService
{
    public interface INguoiDungService : IBaseService<NguoiDung, int>
    {
        NguoiDung LayNguoiDungBangTaiKhoanMatKhau(string tenDangNhap, string matKhau);
    }
}
