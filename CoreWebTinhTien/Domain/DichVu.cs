using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreWebTinhTien.Domain
{
    public class DichVu
    {
        public virtual int Id { get; set; }
        public virtual string Ten { get; set; }
        public virtual string Ma { get; set; }
        public virtual int CongTyId { get; set; }
    }
}
