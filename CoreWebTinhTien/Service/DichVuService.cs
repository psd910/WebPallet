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
    public class DichVuService : BaseService<DichVu, int>, IDichVuService
    {
        public DichVuService(string sessionFactoryConfigPath) : base(sessionFactoryConfigPath)
        { }

    }
}
