using System;
using System.Collections.Generic;

namespace TrinhMinhTan_TourkitApply.Models;

public partial class LoaiSanPham
{
    public int Id { get; set; }

    public string Ten { get; set; } = null!;

    public DateTime NgayNhap { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
