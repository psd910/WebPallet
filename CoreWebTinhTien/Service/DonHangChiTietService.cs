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
    public class DonHangChiTietService : BaseService<DonHangChiTiet, int>, IDonHangChiTietService
    {
        public DonHangChiTietService(string sessionFactoryConfigPath) : base(sessionFactoryConfigPath)
        { }

    }
}
