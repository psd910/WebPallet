using CoreWebTinhTien.BaseServices;
using CoreWebTinhTien.Domain;
using CoreWebTinhTien.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreWebTinhTien.Service
{
    public class NguoiDungService : BaseService<NguoiDung, int>, INguoiDungService
    {
        public NguoiDungService(string sessionFactoryConfigPath) : base(sessionFactoryConfigPath)
        { }

        public NguoiDung LayNguoiDungBangTaiKhoanMatKhau(string tenDangNhap, string matKhau)
        {
            return Query.Where(u => u.TenDangNhap.ToUpper() == tenDangNhap.ToUpper() && u.MatKhau == matKhau).FirstOrDefault();
        }
    }
}
