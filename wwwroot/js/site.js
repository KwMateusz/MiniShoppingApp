document.addEventListener("DOMContentLoaded", function () {
    updateCartCountFromServer();

    const cartButtons = document.querySelectorAll(".add-to-cart");

    cartButtons.forEach(button => {
        button.addEventListener("click", function () {
            const productId = this.getAttribute("data-product-id");

            fetch("/Cart/AddToCart", {
                method: "POST",
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded"
                },
                body: `productId=${productId}`
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        updateCartCount(data.cartCount);
                        showSuccessMessage("Product added to cart!");
                    }
                })
                .catch(error => console.error("Error adding to cart:", error));
        });
    });

    function updateCartCount(count) {
        const cartCounter = document.getElementById("cart-count");
        const cartIcon = document.getElementById("cart-icon");

        if (cartCounter) {
            if (count > 0) {
                cartCounter.textContent = count;
                cartCounter.classList.remove("d-none"); // Show badge
                cartIcon.classList.add("has-items"); // Change icon background
            } else {
                cartCounter.classList.add("d-none"); // Hide badge if empty
                cartIcon.classList.remove("has-items"); // Remove background color
            }
        }
    }

    function updateCartCountFromServer() {
        fetch("/Cart/GetCartCount")
            .then(response => response.json())
            .then(data => {
                updateCartCount(data.cartCount);
            })
            .catch(error => console.error("Error fetching cart count:", error));
    }

    function showSuccessMessage(message) {
        const alertBox = document.createElement("div");
        alertBox.className = "alert alert-success position-fixed top-0 end-0 m-3 fade show";
        alertBox.style.zIndex = "1050";
        alertBox.innerHTML = `${message} <button type="button" class="btn-close" data-bs-dismiss="alert"></button>`;

        document.body.appendChild(alertBox);

        setTimeout(() => {
            alertBox.classList.remove("show");
            setTimeout(() => alertBox.remove(), 500);
        }, 2000);
    }
});