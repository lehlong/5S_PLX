
using PLX5S.CORE.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PLX5S.CORE.Entities.MD
{
    [Table("T_MD_DEVICE")]
    public class TblMdDevice : SoftDeleteEntity
    {
        [Key]
        [Column("ID")]
        public string Id { get; set; }

        [Column("USERNAME")]
        public string UserName { get; set; }

        [Column("DEVICE_ID")]
        public string DeviceId { get; set; }

        [Column("DEVICE_NAME")]
        public string DeviceName { get; set; }

        [Column("OPERATINGSYSTEM")]
        public string OperatingSystem { get; set; }

        [Column("MODEL")]
        public string Model { get; set; }

        [Column("MANUFACTURER")]
        public string Manufacturer { get; set; }

        [Column("OSVERSION")]
        public string OsVersion { get; set; }
        [Column("MAIN_DEVICE")]
        public bool MainDevice { get; set; }
        [Column("ENABLE_LOGIN")]
        public bool EnableLogin { get; set; }



    }
}