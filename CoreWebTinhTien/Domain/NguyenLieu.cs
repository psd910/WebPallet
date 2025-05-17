using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreWebTinhTien.Domain
{
    public class NguyenLieu
    {
        public virtual int Id { get; set; }
        public virtual string Ten { get; set; }
        public virtual string Ma { get; set; }
        public virtual decimal Dai { get; set; }
        public virtual decimal Rong { get; set; }
        public virtual decimal Cao { get; set; }
        public virtual string DonViTinh { get; set; }
        public virtual decimal DinhLuong { get; set; }
        public virtual int SoLuong { get; set; }
        public virtual decimal SoM3 { get; set; }
        public virtual decimal DonGia { get; set; }
        public virtual decimal ThanhTien { get; set; }
        public virtual int CongTyId { get; set; }
    }
}
