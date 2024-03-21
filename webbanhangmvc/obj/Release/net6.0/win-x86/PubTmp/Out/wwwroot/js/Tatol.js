$(document).ready(function () {
    // Lắng nghe sự kiện khi số lượng quantity thay đổi
    $('.quantity input').change(function () {
        // Lấy giá trị số lượng mới
        var newQuantity = $(this).val();
        // Lấy giá sản phẩm
        var pricePerUnit = $(this).closest('tr').find('.shoping__cart__price').text();
        // Tính tổng tiền mới
        var newTotal = parseInt(newQuantity) * parseInt(pricePerUnit);
        // Cập nhật tổng tiền tương ứng
        $(this).closest('tr').find('.shoping__cart__total').text(newTotal + 'VND');

        // Cập nhật tổng tiền của giỏ hàng
        updateTotal();
    });

    // Xử lý sự kiện khi nhấp vào nút xóa
    $('.shoping__cart__item__close .icon_close').click(function () {
        // Xóa hàng khỏi giỏ hàng
        $(this).closest('tr').remove();
        // Tính tổng tiền lại sau khi xóa
        updateTotal();
    });

    // Hàm cập nhật tổng tiền
    function updateTotal() {
        var total = 0;
        $('.shoping__cart__item').each(function () {
            var pricePerUnit = parseInt($(this).find('.shoping__cart__price').text());
            var quantity = parseInt($(this).find('.pro-qty input').val());
            var subtotal = pricePerUnit * quantity;
            total += subtotal;
        });
        // Cập nhật tổng tiền vào phần tổng tiền của trang
        $('#total').text(total + 'VND');
    }
});
