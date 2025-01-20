using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrinhMinhTan_TourkitApply.Dtos;
using TrinhMinhTan_TourkitApply.Models;

namespace TrinhMinhTan_TourkitApply.Controllers
{
    public class HomeController : Controller
    {
        TrinhMinhTanTourkitApplyContext _context = new TrinhMinhTanTourkitApplyContext();

        public HomeController() 
        { 
        
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpGet]
        public IActionResult EditProduct(int idProduct)
        {
            ViewBag.Id = idProduct;
            return View();
        }

        [HttpGet]
        public IActionResult AddTypeProduct()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditTypeProduct(int idTypeProduct)
        {
            var data = await _context.LoaiSanPhams.FirstOrDefaultAsync(x => x.Id == idTypeProduct);

            if (data == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(data);
        }


        [HttpGet]
        public async Task<JsonResult> GetLoaiSanPhams(int page, int pageSize, string nameLoaiSanPhams)
        {
            var totalRecords = await _context.LoaiSanPhams
                .Where(x => string.IsNullOrEmpty(nameLoaiSanPhams) || x.Ten.ToLower().Contains(nameLoaiSanPhams.ToLower()))
                .CountAsync();

            var totalPages = pageSize == -1 ? 1 : (int)Math.Ceiling((double)totalRecords / pageSize);

            var data = await _context.LoaiSanPhams
                .Include(x => x.SanPhams)
                .Where(x => string.IsNullOrEmpty(nameLoaiSanPhams) || x.Ten.ToLower().Contains(nameLoaiSanPhams.ToLower()))
                .OrderBy(x => x.Id)
                .Skip(pageSize == -1 ? 0 : (page - 1) * pageSize)
                .Take(pageSize == -1 ? int.MaxValue : pageSize)
                .Select(x => new
                {
                    x.Id,
                    x.Ten,
                    SoSanPhams = x.SanPhams.Count()
                })
                .ToListAsync();

            return Json(new
            {
                TotalPages = totalPages,
                CurrentPage = page,
                Data = data
            });
        }


        [HttpGet]
        public async Task<JsonResult> GetSanPhams(int page, int pageSize, string nameProduct, int idLoaiSanPhams)
        {

            // Lấy tổng số sản phẩm
            double totalRecords = await _context.SanPhams
                .Include(x => x.LoaiSanPhams)
                .Where(x => (string.IsNullOrEmpty(nameProduct) || x.Ten.ToLower().Contains(nameProduct.ToLower())) &&
                            (idLoaiSanPhams == 0 || x.LoaiSanPhams.Any(x => x.Id == idLoaiSanPhams)))
                .CountAsync();


            // Tính số trang
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            // Lấy dữ liệu sản phẩm theo trang
            var data = await _context.SanPhams
                .Include(x => x.LoaiSanPhams)
                .Where(x => (string.IsNullOrEmpty(nameProduct) || x.Ten.ToLower().Contains(nameProduct.ToLower())) &&
                            (idLoaiSanPhams == 0 || x.LoaiSanPhams.Any(x => x.Id == idLoaiSanPhams)))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.Id,
                    x.Ten,
                    x.Gia,
                    x.NgayNhap,
                    LoaiSanPhams = x.LoaiSanPhams.Select(x => x.Ten).ToList()
                })
                .ToListAsync();

            return Json(new
            {
                TotalPages = totalPages,
                CurrentPage = page,
                Data = data
            });
        }




        [HttpGet]
        public async Task<JsonResult> GetSanPhamsDetail(int idSanPham)
        {
            var data = await _context.SanPhams
             .Include(x => x.LoaiSanPhams)
             .Where(x => x.Id == idSanPham)
             .Select(x => new
             {
                 x.Id,
                 x.Ten,
                 x.Gia,
                 x.NgayNhap,
                 LoaiSanPhams = x.LoaiSanPhams.ToList()
             })
             .FirstOrDefaultAsync();
            return Json(data);
        }



        [HttpPost]
        public async Task<IActionResult> AddSanPhamWithLoai([FromBody] SanPhamWithLoaiDto sanPhamDto)
        {
            // Kiểm tra tên sản phẩm
            if (string.IsNullOrWhiteSpace(sanPhamDto.Ten))
            {
                return BadRequest(new { message = "Tên sản phẩm không được để trống." });
            }

            if (await _context.SanPhams.AnyAsync(sp => sp.Ten.ToLower() == sanPhamDto.Ten.ToLower()))
            {
                return BadRequest(new { message = "Tên sản phẩm đã tồn tại." });
            }

            // Kiểm tra ngày nhập
            if (!DateTime.TryParse(sanPhamDto.NgayNhap, out var ngayNhap))
            {
                return BadRequest(new { message = "Ngày nhập không hợp lệ." });
            }

            // Lấy danh sách LoaiSanPham từ database theo Id
            var loaiSanPhams = await _context.LoaiSanPhams
                .Where(x => sanPhamDto.LoaiSanPhamIds.Contains(x.Id))
                .ToListAsync();

            // Kiểm tra nếu không tìm thấy loại sản phẩm
            if (!loaiSanPhams.Any() && sanPhamDto.LoaiSanPhamIds.Any())
            {
                return BadRequest(new { message = "Không tìm thấy loại sản phẩm tương ứng." });
            }

            // Thêm sản phẩm mới
            var sanPham = new SanPham
            {
                Ten = sanPhamDto.Ten,
                Gia = sanPhamDto.Gia,
                NgayNhap = ngayNhap,
                LoaiSanPhams = loaiSanPhams // Gắn loại sản phẩm vào navigation property
            };

            _context.SanPhams.Add(sanPham);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Thêm sản phẩm thành công." });
        }


        [HttpPost]
        public async Task<IActionResult> EditSanPhamWithLoai([FromBody] SanPhamWithLoaiDto sanPhamDto)
        {
            // Kiểm tra sản phẩm có tồn tại không
            var sanPham = await _context.SanPhams
                .Include(sp => sp.LoaiSanPhams)
                .FirstOrDefaultAsync(sp => sp.Id == sanPhamDto.Id);

            if (sanPham == null)
            {
                return NotFound(new { message = "Sản phẩm không tồn tại." });
            }

            // Kiểm tra tên sản phẩm
            if (string.IsNullOrWhiteSpace(sanPhamDto.Ten))
            {
                return BadRequest(new { message = "Tên sản phẩm không được để trống." });
            }

            // Kiểm tra trùng lặp tên sản phẩm
            if (await _context.SanPhams.AnyAsync(sp => sp.Ten.ToLower() == sanPhamDto.Ten.ToLower() && sp.Id != sanPhamDto.Id))
            {
                return BadRequest(new { message = "Tên sản phẩm đã tồn tại." });
            }

            // Kiểm tra ngày nhập
            if (!DateTime.TryParse(sanPhamDto.NgayNhap, out var ngayNhap))
            {
                return BadRequest(new { message = "Ngày nhập không hợp lệ." });
            }

            // Cập nhật thông tin sản phẩm
            sanPham.Ten = sanPhamDto.Ten;
            sanPham.Gia = sanPhamDto.Gia;
            sanPham.NgayNhap = ngayNhap;

            // Cập nhật loại sản phẩm
            sanPham.LoaiSanPhams.Clear();
            var loaiSanPhams = await _context.LoaiSanPhams
                .Where(x => sanPhamDto.LoaiSanPhamIds.Contains(x.Id))
                .ToListAsync();

            if (!loaiSanPhams.Any() && sanPhamDto.LoaiSanPhamIds.Any())
            {
                return BadRequest(new { message = "Không tìm thấy loại sản phẩm tương ứng." });
            }

            sanPham.LoaiSanPhams.Clear();

            foreach (var loaiSanPham in loaiSanPhams)
            {
                sanPham.LoaiSanPhams.Add(loaiSanPham);
            }

            _context.SanPhams.Update(sanPham);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật sản phẩm thành công." });
        }



        [HttpPost]
        public async Task<IActionResult> DelSanPhams(int idSanPham)
        {
            // Tìm sản phẩm theo Id
            var sanPham = await _context.SanPhams.FirstOrDefaultAsync(x => x.Id == idSanPham);
            if (sanPham == null)
            {
                return NotFound(new { success = false, message = "Sản phẩm không tồn tại." });
            }

            try
            {
                // Xóa sản phẩm
                _context.SanPhams.Remove(sanPham);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Xóa sản phẩm thành công!" });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khi xóa
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi xóa sản phẩm.", error = ex.Message });
            }
        }



        [HttpPost]
        public async Task<IActionResult> AddLoaiSanPham([FromBody] LoaiSanPhamDto loaiSanPhamDto)
        {
            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(loaiSanPhamDto.Ten))
            {
                return BadRequest(new { success = false, message = "Tên loại sản phẩm không được để trống." });
            }

            if (await _context.LoaiSanPhams.AnyAsync(x => x.Ten.ToLower() == loaiSanPhamDto.Ten.ToLower()))
            {
                return BadRequest(new { success = false, message = "Tên loại sản phẩm đã tồn tại." });
            }

            try
            {
                // Tạo mới đối tượng LoaiSanPham
                var loaiSanPham = new LoaiSanPham
                {
                    Ten = loaiSanPhamDto.Ten,
                    NgayNhap = loaiSanPhamDto.NgayNhap
                };

                // Lưu vào database
                _context.LoaiSanPhams.Add(loaiSanPham);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Thêm loại sản phẩm thành công!" });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi thêm loại sản phẩm.", error = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> EditLoaiSanPham([FromBody] LoaiSanPhamDto loaiSanPhamDto)
        {
            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(loaiSanPhamDto.Ten))
            {
                return BadRequest(new { success = false, message = "Tên loại sản phẩm không được để trống." });
            }

            // Kiểm tra loại sản phẩm có tồn tại không
            var loaiSanPham = await _context.LoaiSanPhams.FirstOrDefaultAsync(ls => ls.Id == loaiSanPhamDto.Id);
            if (loaiSanPham == null)
            {
                return NotFound(new { success = false, message = "Loại sản phẩm không tồn tại." });
            }

            // Kiểm tra trùng tên (ngoại trừ chính nó)
            if (await _context.LoaiSanPhams.AnyAsync(ls => ls.Ten.ToLower() == loaiSanPhamDto.Ten.ToLower() && ls.Id != loaiSanPhamDto.Id))
            {
                return BadRequest(new { success = false, message = "Tên loại sản phẩm đã tồn tại." });
            }

            try
            {
                // Cập nhật thông tin loại sản phẩm
                loaiSanPham.Ten = loaiSanPhamDto.Ten;
                loaiSanPham.NgayNhap = loaiSanPhamDto.NgayNhap;

                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Cập nhật loại sản phẩm thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi cập nhật loại sản phẩm.", error = ex.Message });
            }
        }




        [HttpPost]
        public async Task<IActionResult> DelLoaiSanPham(int id)
        {
            // Kiểm tra loại sản phẩm có tồn tại không
            var loaiSanPham = await _context.LoaiSanPhams.FirstOrDefaultAsync(x => x.Id == id);
            if (loaiSanPham == null)
            {
                return NotFound(new { success = false, message = "Loại sản phẩm không tồn tại." });
            }

            try
            {
                // Xóa loại sản phẩm
                _context.LoaiSanPhams.Remove(loaiSanPham);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Xóa loại sản phẩm thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi xóa loại sản phẩm.", error = ex.Message });
            }
        }





    }
}
