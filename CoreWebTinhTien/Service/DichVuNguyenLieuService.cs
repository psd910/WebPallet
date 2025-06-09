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
    public class DichVuNguyenLieuService : BaseService<DichVuNguyenLieu, int>, IDichVuNguyenLieuService
    {
        public DichVuNguyenLieuService(string sessionFactoryConfigPath) : base(sessionFactoryConfigPath)
        { }

        public List<DichVuNguyenLieu> GetByDichVuId(int dichVuId)
        {
            return Query.Where(x => x.DichVuId == dichVuId).ToList();
        }
        public DichVuNguyenLieu GetByNguyenLieuId(int nguyenLieuId)
        {
            return Query.Where(x => x.NguyenLieuId == nguyenLieuId).FirstOrDefault();
        }
    }
}
