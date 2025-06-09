using CoreWebTinhTien.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTinhTien.Models
{
    public class DichVuModel
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public List<DichVuNguyenLieuModel> DanhSachNguyenLieu { get; set; } // danh sách nguyên liệu được chọn
       // public List<NguyenLieu> DanhSachNguyenLieu { get; set; } // để hiển thị trong View
    }
}