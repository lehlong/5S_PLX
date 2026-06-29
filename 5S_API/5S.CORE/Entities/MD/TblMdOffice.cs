using PLX5S.CORE.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLX5S.CORE.Entities.MD
{
    [Table("T_MD_OFFICE")]
    public class TblMdOffice : SoftDeleteEntity
    {
        [Key]
        [Column("ID")]
        public string Id { get; set; }

        [Column("NAME", TypeName = "NVARCHAR(255)")]
        public string Name { get; set; }

        [Column("PHONE_NUMBER")]
        public string? PhoneNumber { get; set; }

        [Column("THU_TRUONG")]
        public string ThuTruong { get; set; }

        [Column("NGUOI_PHU_TRACH")]
        public string NguoiPhuTrach { get; set; }

        [Column("KINH_DO")]
        public string? KinhDo { get; set; }

        [Column("VI_DO")]
        public string? ViDo { get; set; }

        [Column("ADDRESS")]
        public string? Address { get; set; }

        [Column("TRANG_THAI", TypeName = "BIT")]
        public bool? TrangThai { get; set; }

    }
}
