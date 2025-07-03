using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PLX5S.CORE.Common;

namespace PLX5S.CORE.Entities.MD
{
    [Table("T_MD_WARE_HOUSE")]
public class TblMdWareHouse : SoftDeleteEntity
{
    [Key]
    [Column("ID")]
    public string Id { get; set; }

    [Column("NAME", TypeName = "NVARCHAR(255)")]
    public string Name { get; set; }

    [Column("PHONE_NUMBER")]
    public string? PhoneNumber { get; set; }

    [Column("TRUONG_KHO")]
    public string TruongKho { get; set; }

    [Column("NGUOI_PHU_TRACH")]
    public string NguoiPhuTrach { get; set; }

    [Column("KINH_DO")]
    public string? KinhDo { get; set; }

    [Column("ADDRESS")]
    public string? Address { get; set; }

    [Column("VI_DO")]
    public string? ViDo { get; set; }

    [Column("TRANG_THAI_KHO", TypeName = "BIT")]
    public bool? TrangThaiKho { get; set; }
    }
}
