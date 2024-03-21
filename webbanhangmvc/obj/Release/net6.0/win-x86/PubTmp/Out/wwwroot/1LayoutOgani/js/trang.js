document.addEventListener("DOMContentLoaded", function () {
    // Lấy tất cả các phần tử có class 'page'
    var pages = document.querySelectorAll(".product__pagination .page");

    // Lặp qua từng phần tử và thêm sự kiện click
    pages.forEach(function (page) {
        page.addEventListener("click", function (event) {
            // Ngăn chặn hành vi mặc định của thẻ <a>
            event.preventDefault();

            // Thêm lớp 'active' cho phần tử được nhấp vào
            this.classList.add("active");

            // Ẩn số trang đầu tiên khi active
            var firstPage = document.querySelector(".product__pagination .page:first-child");
            firstPage.style.display = "none";
        });
    });
});
