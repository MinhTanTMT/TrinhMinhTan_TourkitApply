using System;
using System.Collections.Generic;

namespace TrinhMinhTan_TourkitApply.Models;

public partial class SanPham
{
    public int Id { get; set; }

    public string Ten { get; set; } = null!;

    public decimal Gia { get; set; }

    public DateTime NgayNhap { get; set; }

    public virtual ICollection<LoaiSanPham> LoaiSanPhams { get; set; } = new List<LoaiSanPham>();
}
