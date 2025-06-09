using CoreWebTinhTien.Domain;
using CoreWebTinhTien.BaseServices;
using System.Collections.Generic;

namespace CoreWebTinhTien.IService
{
    public interface IDichVuNguyenLieuService : IBaseService<DichVuNguyenLieu, int>
    {
        List<DichVuNguyenLieu> GetByDichVuId(int dichVuId);
        DichVuNguyenLieu GetByNguyenLieuId(int nguyenLieuId);
    }
}
