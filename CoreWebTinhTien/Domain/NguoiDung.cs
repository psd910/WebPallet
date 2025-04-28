using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreWebTinhTien.Domain
{
    public class NguoiDung
    {
        public virtual int Id { get; set; }
        public virtual string TenDangNhap { get; set; }
        public virtual string MatKhau { get; set; }
        public virtual string Email { get; set; }
        public virtual int CongTyId { get; set; }
    }
}
