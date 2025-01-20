-- Tạo database
CREATE DATABASE TrinhMinhTan_TourkitApply;
GO

-- Sử dụng database vừa tạo
USE TrinhMinhTan_TourkitApply;
GO

-- Tạo bảng Loại Sản Phẩm
CREATE TABLE LoaiSanPham (
    Id INT IDENTITY(1,1) PRIMARY KEY, -- Khóa chính
    Ten NVARCHAR(255) NOT NULL UNIQUE, -- Tên loại sản phẩm, không trùng lặp
    NgayNhap DATETIME NOT NULL -- Ngày nhập
);

-- Tạo bảng Sản Phẩm
CREATE TABLE SanPham (
    Id INT IDENTITY(1,1) PRIMARY KEY, -- Khóa chính
    Ten NVARCHAR(255) NOT NULL, -- Tên sản phẩm
    Gia DECIMAL(18, 2) NOT NULL, -- Giá sản phẩm
    NgayNhap DATETIME NOT NULL -- Ngày nhập
);

-- Tạo bảng trung gian cho mối quan hệ nhiều-nhiều
CREATE TABLE SanPham_LoaiSanPham (
    SanPhamId INT NOT NULL, -- Khóa ngoại đến bảng Sản Phẩm
    LoaiSanPhamId INT NOT NULL, -- Khóa ngoại đến bảng Loại Sản Phẩm
    PRIMARY KEY (SanPhamId, LoaiSanPhamId), -- Định nghĩa khóa chính kết hợp
    CONSTRAINT FK_SanPham FOREIGN KEY (SanPhamId) REFERENCES SanPham(Id) ON DELETE CASCADE,
    CONSTRAINT FK_LoaiSanPham FOREIGN KEY (LoaiSanPhamId) REFERENCES LoaiSanPham(Id) ON DELETE CASCADE
);
