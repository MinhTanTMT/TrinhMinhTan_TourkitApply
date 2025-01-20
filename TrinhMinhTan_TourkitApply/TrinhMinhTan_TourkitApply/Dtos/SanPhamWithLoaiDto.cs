namespace TrinhMinhTan_TourkitApply.Dtos
{
    public class SanPhamWithLoaiDto
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public decimal Gia { get; set; }
        public string NgayNhap { get; set; }
        public List<int> LoaiSanPhamIds { get; set; } = new List<int>();
    }

}
