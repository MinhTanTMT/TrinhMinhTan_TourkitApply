$(document).ready(function () {
    // Write your JavaScript code.
    const currentPath = window.location.pathname;

    if (currentPath.includes('/Home/EditProduct') || currentPath.includes('/Home/AddProduct')) {

        let currentPageTypeProduct = 1;
        let pageSizeType = -1;

        // Load kiểu sản phẩm
        function loadTypeProductOption(page, nameLoaiSanPhams) {
            $.ajax({
                url: '/Home/GetLoaiSanPhams',
                type: 'GET',
                data: { page: page, pageSize: pageSizeType, nameLoaiSanPhams: nameLoaiSanPhams },
                success: function (dataGetLoaiSanPhams) {
                    var typeProductOption = $('#typeProduct');
                    typeProductOption.empty();
                    console.log(dataGetLoaiSanPhams.data);
                    $.each(dataGetLoaiSanPhams.data, function (index, item) {
                        var option = `<option value="${item.id}">${item.ten}</option>`;
                        typeProductOption.append(option);
                    });
                    currentPageTypeProducts = dataGetLoaiSanPhams.currentPage;
                },
                error: function (err) {
                    console.error("Lỗi:", err);
                }
            });
        }
        $('#searchInputTypeProduct').on('input', function () {
            loadTypeProductOption(currentPageTypeProducts, $(this).val());
        });

        loadTypeProductOption(currentPageTypeProduct, $('#searchInputTypeProduct').val());

        const selectedTypeProductList = []; // Danh sách loại sản phẩm đã chọn

        // Xóa loại sản phẩm khỏi danh sách
        $('#selectedTypeProductList').on('click', '.remove-item', function () {
            const id = $(this).data('id');
            const index = selectedTypeProductList.findIndex(item => item.id == id);
            if (index > -1) {
                selectedTypeProductList.splice(index, 1);
            }
            renderSelectedTypeProductList();
        });

        // Thêm loại sản phẩm vào danh sách
        $('#addToList').on('click', function () {
            const selectedOption = $('#typeProduct option:selected');
            const id = selectedOption.val();
            const ten = selectedOption.text();

            if (!id || selectedTypeProductList.find(item => item.id === id)) {
                alert('Loại sản phẩm đã được thêm hoặc không hợp lệ!');
                return;
            }

            selectedTypeProductList.push({ id, ten });
            renderSelectedTypeProductList();
        });


        // Gen Lại danh sách
        function renderSelectedTypeProductList() {
            const listElement = $('#selectedTypeProductList');
            listElement.empty();

            selectedTypeProductList.forEach(item => {
                const listItem = `
                <li class="list-group-item d-flex justify-content-between align-items-center">
                    ID: ${item.id}, Tên: ${item.ten}
                    <button class="btn btn-danger btn-sm remove-item" data-id="${item.id}">Xóa</button>
                </li>
            `;
                listElement.append(listItem);
            });
        }


        /////////////////////////////////////////////////////////////////

        if (currentPath.includes('/Home/AddProduct')) {


            // Thêm sản phẩm
            $('#addProduct').on('click', function () {
                const ten = $('#tenSanPham').val();
                const gia = parseFloat($('#giaSanPham').val());
                const ngayNhap = $('#ngayNhap').val();
                const loaiSanPhamIds = selectedTypeProductList.map(item => parseInt(item.id));

                if (!ten || !gia || !ngayNhap || loaiSanPhamIds.length === 0) {
                    alert('Vui lòng điền đầy đủ thông tin và chọn ít nhất một loại sản phẩm!');
                    return;
                }

                const data = {
                    ten,
                    gia,
                    ngayNhap,
                    loaiSanPhamIds
                };

                $.ajax({
                    url: '/Home/AddSanPhamWithLoai',
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify(data),
                    success: function (response) {
                        alert(response.message || 'Thêm sản phẩm thành công!');
                        $('#tenSanPham').val('');
                        $('#giaSanPham').val('');
                        $('#ngayNhap').val('');
                        selectedTypeProductList.length = 0;
                        window.location.href = '/Home/Index';
                    },
                    error: function (err) {
                        console.error('Lỗi:', err);
                        alert(err.responseJSON?.message || 'Thêm sản phẩm thất bại!');
                    }
                });
            });


            console.log('Đang ở trang Add Product!');


            /////////////////////////////////////////////////////////////////
        } else if (currentPath.includes('/Home/EditProduct')) {

            // Load thông tin sản phẩm để chỉnh sửa
            const productId = $('#productId').val();
            function loadProductDetails() {
                $.ajax({
                    url: `/Home/GetSanPhamsDetail`,
                    type: 'GET',
                    data: { idSanPham: productId },
                    success: function (data) {

                        console.log(data);


                        if (data) {
                            $('#tenSanPham').val(data.ten);
                            $('#giaSanPham').val(data.gia);

                            const formattedDate = data.ngayNhap.split('T')[0];
                            $('#ngayNhap').val(formattedDate);

                            data.loaiSanPhams.forEach(loai => {
                                selectedTypeProductList.push({ id: loai.id, ten: loai.ten });
                            });

                            renderSelectedTypeProductList();
                        } else {
                            alert('Không tìm thấy sản phẩm!');
                        }
                    },
                    error: function (err) {
                        console.error('Lỗi:', err);
                        alert('Không thể tải thông tin sản phẩm!');
                    }
                });
            }


            // Lưu sản phẩm sau khi chỉnh sửa
            $('#saveProduct').on('click', function () {
                const ten = $('#tenSanPham').val();
                const gia = parseFloat($('#giaSanPham').val());
                const ngayNhap = $('#ngayNhap').val();
                const loaiSanPhamIds = selectedTypeProductList.map(item => parseInt(item.id));

                if (!ten || !gia || !ngayNhap || loaiSanPhamIds.length === 0) {
                    alert('Vui lòng điền đầy đủ thông tin và chọn ít nhất một loại sản phẩm!');
                    return;
                }

                const data = {
                    id: productId,
                    ten,
                    gia,
                    ngayNhap,
                    loaiSanPhamIds
                };

                $.ajax({
                    url: '/Home/EditSanPhamWithLoai',
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify(data),
                    success: function (response) {
                        alert(response.message || 'Cập nhật sản phẩm thành công!');
                        window.location.href = '/Home/Index';

                    },
                    error: function (err) {
                        console.error('Lỗi:', err);
                        alert(err.responseJSON?.message || 'Cập nhật sản phẩm thất bại!');
                    }
                });
            });

            loadProductDetails();

        } else {
            console.log('Trang không xác định!');
        }

    } else {

        if (currentPath.includes('/Home/AddTypeProduct')) {

            $('#addLoaiSanPhamForm').on('submit', function (e) {
                e.preventDefault();

                const tenLoaiSanPham = $('#tenLoaiSanPham').val();
                const ngayNhap = $('#ngayNhap').val();

                if (!tenLoaiSanPham || !ngayNhap) {
                    alert('Vui lòng điền đầy đủ thông tin!');
                    return;
                }

                const data = {
                    ten: tenLoaiSanPham,
                    ngayNhap: ngayNhap
                };

                $.ajax({
                    url: '/Home/AddLoaiSanPham',
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify(data),
                    success: function (response) {
                        if (response.success) {
                            alert(response.message);
                            window.location.href = '/Home/Index';
                        } else {
                            alert(response.message);
                        }
                    },
                    error: function (xhr) {
                        console.error('Lỗi:', xhr.responseJSON);
                        alert(xhr.responseJSON?.message || 'Có lỗi xảy ra khi thêm loại sản phẩm!');
                    }
                });
            });

        } else if (currentPath.includes('/Home/EditTypeProduct')) {

            $('#editLoaiSanPhamForm').on('submit', function (e) {
                e.preventDefault();

                const id = $('#editLoaiSanPhamId').val();
                const ten = $('#editTenLoaiSanPham').val();
                const ngayNhap = $('#editNgayNhap').val();

                if (!ten || !ngayNhap) {
                    alert('Vui lòng điền đầy đủ thông tin!');
                    return;
                }

                const data = { id, ten, ngayNhap };

                $.ajax({
                    url: '/Home/EditLoaiSanPham',
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify(data),
                    success: function (response) {
                        if (response.success) {
                            alert(response.message);
                            window.location.href = '/Home/Index';
                        } else {
                            alert(response.message);
                        }
                    },
                    error: function (xhr) {
                        console.error('Lỗi:', xhr.responseJSON);
                        alert(xhr.responseJSON?.message || 'Có lỗi xảy ra khi cập nhật loại sản phẩm!');
                    }
                });
            });


        } else {
            console.log('Trang không xác định!');
        }
    }

});
