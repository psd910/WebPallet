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
    public class DonHangService : BaseService<DonHang, int>, IDonHangService
    {
        public DonHangService(string sessionFactoryConfigPath) : base(sessionFactoryConfigPath)
        { }

    }
}
