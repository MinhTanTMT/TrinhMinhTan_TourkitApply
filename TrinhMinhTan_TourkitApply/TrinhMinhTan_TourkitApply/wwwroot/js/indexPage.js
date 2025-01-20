// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


let currentPageProducts = 1;
let currentPageTypeProduct = 1;
let nameProduct = '';
let nameLoaiSanPhams = '';
let idLoaiSanPhams = 0;
const pageSizeProducts = 10; 
const pageSizeTypeProduct = 10; 

let isPageProduct = true;


// Hàm tải dữ liệu sản phẩm
function loadProducts(page, nameProduct, idLoaiSanPhams) {
    $.ajax({
        url: '/Home/GetSanPhams',
        type: 'GET',
        data: { page: page, pageSize: pageSizeProducts, nameProduct: nameProduct, idLoaiSanPhams: idLoaiSanPhams },
        success: function (dataGetSanPhams) {
            var productTable = $('#productTable tbody');
            productTable.empty();

            console.log(dataGetSanPhams.data);

            $.each(dataGetSanPhams.data, function (index, item) {
                var row = `
                            <tr>
                                <td>${item.id}</a></td>
                                <td>${item.ten}</td>
                                <td>${item.gia}</td>
                                <td>${item.loaiSanPhams}</td>
                                <td>${item.ngayNhap}</td>
                                <td>
                                    <a href="/Home/EditProduct?idProduct=${item.id}" class="btn btn-warning btn-sm">
                                        <i class="fas fa-edit"></i>
                                    </a>
                                </td>

                                <td>
                                    <button class="btn btn-danger btn-sm delete-product" data-id="${item.id}">
                                        <i class="fas fa-times"></i> Xóa
                                    </button>
                                </td>
                            </tr>
                        `;
                productTable.append(row);
            });

            // Cập nhật trang hiện tại
            currentPageProducts = dataGetSanPhams.currentPage;
            $('#currentPage').text(currentPageProducts);

            // Vô hiệu hóa nút nếu ở trang đầu tiên hoặc cuối cùng
            $('#prevPage').prop('disabled', currentPageProducts === 1);
            $('#nextPage').prop('disabled', currentPageProducts === dataGetSanPhams.totalPages);
        },
        error: function (err) {
            console.error("Lỗi:", err);
        }
    });
}
// Load kiểu sản phẩm
function loadTypeProductOption(page, nameLoaiSanPhams) {
    let pageSizeType;
    if (isPageProduct) {
        pageSizeType = -1;
    } else {
        pageSizeType = pageSizeTypeProduct;
    }
    $.ajax({
        url: '/Home/GetLoaiSanPhams',
        type: 'GET',
        data: { page: page, pageSize: pageSizeType, nameLoaiSanPhams: nameLoaiSanPhams },
        success: function (dataGetLoaiSanPhams) {
            if (isPageProduct) {
                var typeProductOption = $('#typeProduct');
                typeProductOption.empty();
                console.log(dataGetLoaiSanPhams.data);
                typeProductOption.append('<option value="0">Tất cả</option>');
                $.each(dataGetLoaiSanPhams.data, function (index, item) {
                    var option = `<option value="${item.id}">${item.ten}</option>`;
                    typeProductOption.append(option);
                });
                currentPageTypeProducts = dataGetLoaiSanPhams.currentPage;
            } else {
                var typeProductTable = $('#typeProductTable tbody');
                typeProductTable.empty();
                $.each(dataGetLoaiSanPhams.data, function (index, item) {
                    var row = `
                                <tr>
                                    <td>${item.id}</a></td>
                                    <td>${item.ten}</td>
                                    <td>${item.soSanPhams}</td>
                                    <td>
                                        <a href="/Home/EditTypeProduct?idTypeProduct=${item.id}" class="btn btn-warning btn-sm">
                                            <i class="fas fa-edit"></i>
                                        </a>
                                    </td>

                                    <td>
                                        <button class="btn btn-danger btn-sm delete-typeProduct" data-id="${item.id}" data-sosanpham="${item.soSanPhams}">
                                            <i class="fas fa-times"></i> Xóa
                                        </button>
                                    </td>
                                </tr>
                            `;
                    typeProductTable.append(row);
                });

                currentPageTypeProducts = dataGetLoaiSanPhams.currentPage;
                $('#currentPage').text(currentPageTypeProducts);

                // Vô hiệu hóa nút nếu ở trang đầu tiên hoặc cuối cùng
                $('#prevPage').prop('disabled', currentPageTypeProducts === 1);
                $('#nextPage').prop('disabled', currentPageTypeProducts === dataGetLoaiSanPhams.totalPages);
            }
        },
        error: function (err) {
            console.error("Lỗi:", err);
        }
    });
}

$('#prevPage').on('click', function () {
    if (isPageProduct) {
        if (currentPageProducts > 1) {
            loadProducts(currentPageProducts - 1, $('#searchInput').val(), $('#typeProduct').val());
        }
    } else {
        if (currentPageTypeProducts > 1) {
            loadTypeProductOption(currentPageTypeProducts - 1, $('#searchInput').val());
        }
    }
});

$('#nextPage').on('click', function () {
    if (isPageProduct) {
        loadProducts(currentPageProducts + 1, $('#searchInput').val(), $('#typeProduct').val());
    } else {
        loadTypeProductOption(currentPageTypeProducts + 1, $('#searchInput').val());
    }
});

$('#hiddenTable').on('click', function () {

    if (isPageProduct) {
        isPageProduct = false;
    } else {
        isPageProduct = true;
    }

    if (isPageProduct) {
        $('#productTable').css('display', 'table');
        $('#typeProductTable').css('display', 'none');
        $('#productTypeSelectionDiv').css('display', 'block');
        $('#btnAddProduct').css('display', 'table');
        $('#btnAddTypeProduct').css('display', 'none');
        loadTypeProductOption(currentPageTypeProducts, nameLoaiSanPhams);
        loadProducts(currentPageProducts, $('#searchInput').val(), $('#typeProduct').val());
    } else {
        $('#productTable').css('display', 'none');
        $('#typeProductTable').css('display', 'table');
        $('#productTypeSelectionDiv').css('display', 'none');
        $('#btnAddProduct').css('display', 'none');
        $('#btnAddTypeProduct').css('display', 'table');
        loadTypeProductOption(currentPageTypeProduct, $('#searchInput').val());
    }
})

$('#searchInput').on('input', function () {
    if (isPageProduct) {
        loadProducts(currentPageProducts, $('#searchInput').val(), $('#typeProduct').val());
    } else {
        loadTypeProductOption(currentPageTypeProducts, $('#searchInput').val());
    }
})


$('#typeProduct').on('change', function () {
    loadProducts(currentPageProducts, $('#searchInput').val(), $('#typeProduct').val());
})

$('#searchInputTypeProduct').on('input', function () {
    loadTypeProductOption(currentPageTypeProducts, $(this).val());
})

loadTypeProductOption(currentPageTypeProduct, $('#searchInputTypeProduct').val()); // kiểm tra lại lúc dầu nó là $('#searchInput').val()
loadProducts(currentPageProducts, $('#searchInput').val(), $('#typeProduct').val());


// Hàm xóa sản phẩm 
$(document).on('click', '.delete-product', function () {
    const productId = $(this).data('id');
    const confirmDelete = confirm('Bạn có chắc chắn muốn xóa sản phẩm này không?');

    if (!confirmDelete) return;

    $.ajax({
        url: '/Home/DelSanPhams',
        type: 'POST',
        data: { idSanPham: productId },
        success: function (response) {
            if (response.success) {
                alert(response.message);
                loadProducts(currentPageProducts, $('#searchInput').val(), $('#typeProduct').val());
            } else {
                alert(response.message);
            }
        },
        error: function (xhr) {
            console.error('Lỗi:', xhr.responseJSON);
            alert(xhr.responseJSON?.message || 'Có lỗi xảy ra khi xóa sản phẩm!');
        }
    });
});

// Hàm xóa loại sản phẩm
$(document).on('click', '.delete-typeProduct', function () {
    const id = $(this).data('id');
    const soSanPhams = $(this).data('sosanpham');
    const confirmDelete = confirm('Bạn có chắc chắn muốn xóa loại sản phẩm này không?');

    if (!confirmDelete) return;

    if (soSanPhams > 0) {
        alert('Số lượng sản phẩm phải bằng 0');
        return;
    }

    $.ajax({
        url: '/Home/DelLoaiSanPham',
        type: 'POST',
        data: { id },
        success: function (response) {
            if (response.success) {
                alert(response.message);
                loadTypeProductOption(currentPageTypeProduct, $('#searchInputTypeProduct').val());
            } else {
                alert(response.message);
            }
        },
        error: function (xhr) {
            console.error('Lỗi:', xhr.responseJSON);
            alert(xhr.responseJSON?.message || 'Có lỗi xảy ra khi xóa loại sản phẩm!');
        }
    });
});


