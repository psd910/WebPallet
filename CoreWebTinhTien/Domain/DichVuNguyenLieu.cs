using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreWebTinhTien.Domain
{
    public class DichVuNguyenLieu
    {
        public virtual int Id { get; set; }
        public virtual int DichVuId { get; set; }
        public virtual int NguyenLieuId { get; set; }
        public virtual int SoLuong { get; set; }
    }
}
