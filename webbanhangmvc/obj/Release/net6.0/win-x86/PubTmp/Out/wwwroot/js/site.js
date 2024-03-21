////$(document).ready(function () {
////    // Lắng nghe sự kiện khi số lượng quantity thay đổi
////    $('.quantity .pro-qty input').change(function () {
////        updateQuantityAndTotal($(this));
////    });

////    // Lắng nghe sự kiện khi nhấn nút '+' hoặc '-'
////    $('.quantity .pro-qty .inc, .quantity .pro-qty .dec').click(function () {
////        var inputQuantity = $(this).siblings('input');
////        // Lấy giá trị hiện tại của input số lượng
////        var currentQuantity = parseInt(inputQuantity.val());
////        // Lấy giá trị của dấu '+' hoặc '-'
////        var operation = $(this).hasClass('inc') ? 1 : -1;
////        // Tăng hoặc giảm giá trị số lượng
////        var newQuantity = currentQuantity + operation;
////        // Giới hạn số lượng không âm và không quá 1 khi trừ
////        newQuantity = Math.max(1, newQuantity);
////        // Cập nhật giá trị mới vào input số lượng
////        inputQuantity.val(newQuantity);
////        // Gọi hàm cập nhật tổng
////        updateQuantityAndTotal(inputQuantity);
////    });

////    // Xử lý sự kiện khi nhấp vào nút xóa
////    $('.shoping__cart__item__close .icon_close').click(function () {
////        // Xóa hàng khỏi giỏ hàng
////        $(this).closest('tr').remove();
////        // Tính tổng tiền lại sau khi xóa
////        updateTotal();
////    });

////    // Hàm cập nhật số lượng và tính tổng
////    function updateQuantityAndTotal(inputQuantity) {
////        var quantity = parseInt(inputQuantity.val());
////        // Lấy giá sản phẩm
////        var pricePerUnit = parseInt(inputQuantity.closest('tr').find('.shoping__cart__price').text());
////        // Tính tổng tiền mới
////        var newTotal = quantity * pricePerUnit;
////        // Cập nhật tổng tiền tương ứng
////        inputQuantity.closest('tr').find('.shoping__cart__total').text(newTotal + 'VND');

////        // Cập nhật tổng tiền của giỏ hàng
////        updateTotal();
////    }

////    // Hàm cập nhật tổng tiền
////    function updateTotal() {
////        var total = 0;
////        $('.shoping__cart__item').each(function () {
////            var pricePerUnit = parseInt($(this).find('.shoping__cart__price').text());
////            var quantity = parseInt($(this).find('.pro-qty input').val());
////            var subtotal = pricePerUnit * quantity;
////            total += subtotal;
////        });
////        // Cập nhật tổng tiền vào phần tổng tiền của trang
////        $('.shoping__checkout ul li span').text(total + 'VND');
////    }
////});
