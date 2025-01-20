using System;

namespace TrinhMinhTan_TourkitApply.Models
{
    public class SeedData
    {
        public static void Seed(TrinhMinhTanTourkitApplyContext context)
        {
            if (!context.LoaiSanPhams.Any() && !context.SanPhams.Any())
            {
                // Tạo 20 loại sản phẩm
                var loaiSanPhams = Enumerable.Range(1, 20).Select(i => new LoaiSanPham
                {
                    Ten = $"Loại sản phẩm {i}",
                    NgayNhap = DateTime.Now.AddDays(-i)
                }).ToList();
                context.LoaiSanPhams.AddRange(loaiSanPhams);

                // Tạo 10,000 sản phẩm
                var sanPhams = Enumerable.Range(1, 10000).Select(i => new SanPham
                {
                    Ten = $"Sản phẩm {i}",
                    Gia = new Random().Next(1000, 1000000), 
                    NgayNhap = DateTime.Now.AddDays(-new Random().Next(0, 365)) 
                }).ToList();
                context.SanPhams.AddRange(sanPhams);

                context.SaveChanges(); 

                var random = new Random();
                foreach (var sanPham in sanPhams)
                {
                    var randomLoaiSanPhams = loaiSanPhams.OrderBy(_ => random.Next()).Take(random.Next(1, 4)).ToList();
                    foreach (var loaiSanPham in randomLoaiSanPhams)
                    {
                        sanPham.LoaiSanPhams.Add(loaiSanPham); 
                    }
                }

                context.SaveChanges(); // Lưu tất cả thay đổi
            }
        }
    }

}
