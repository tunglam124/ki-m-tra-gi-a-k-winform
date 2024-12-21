namespace De01.models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SINHVIEN")]
    public partial class SINHVIEN
    {
        [Key]
        [StringLength(6)]
        public string MaSV { get; set; }

        [Required]
        [StringLength(40)]
        public string HoTenSV { get; set; }

        [Required]
        [StringLength(3)]
        public string MaLop { get; set; }

        public DateTime NgaySinh { get; set; }

        public virtual LOP LOP { get; set; }
    }
}
