document.addEventListener("DOMContentLoaded", function () {
    updateCartCountFromServer();
    updateProductQuantities();

    document.addEventListener("click", function (event) {
        if (event.target.classList.contains("add-to-cart")) {
            let productId = event.target.getAttribute("data-product-id");
            addToCart(productId);
        } else if (event.target.classList.contains("increase-qty")) {
            let productId = event.target.getAttribute("data-product-id");
            addToCart(productId);
        } else if (event.target.classList.contains("decrease-qty")) {
            let productId = event.target.getAttribute("data-product-id");
            removeFromCart(productId);
        }
    });

    function addToCart(productId) {
        fetch("/Cart/AddToCart", {
            method: "POST",
            headers: { "Content-Type": "application/x-www-form-urlencoded" },
            body: `productId=${productId}`
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    updateCartCount(data.cartCount);
                    updateProductQuantityDisplay(productId, data.productQuantity);
                    showSuccessMessage("Product added to cart!");
                }
            })
            .catch(error => console.error("Error adding to cart:", error));
    }

    function removeFromCart(productId) {
        fetch("/Cart/RemoveFromCart", {
            method: "POST",
            headers: { "Content-Type": "application/x-www-form-urlencoded" },
            body: `productId=${productId}`
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    updateCartCount(data.cartCount);
                    updateProductQuantityDisplay(productId, data.productQuantity);
                }
            })
            .catch(error => console.error("Error removing from cart:", error));
    }

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

    function updateProductQuantities() {
        document.querySelectorAll("[id^='cart-controls-']").forEach(element => {
            let productId = element.id.replace("cart-controls-", "");
            fetch(`/Cart/GetProductQuantity?productId=${productId}`)
                .then(response => response.json())
                .then(data => updateProductQuantityDisplay(productId, data.productQuantity))
                .catch(error => console.error("Error fetching product quantity:", error));
        });
    }

    function updateProductQuantityDisplay(productId, quantity) {
        let container = document.getElementById(`cart-controls-${productId}`);
        if (quantity > 0) {
            container.innerHTML = `
                <div class="d-flex align-items-center justify-content-between border p-2">
                    <button class="btn btn-danger decrease-qty" data-product-id="${productId}">-</button>
                    <span class="mx-2 bg-white px-3 py-1 border rounded">${quantity}</span>
                    <button class="btn btn-primary increase-qty" data-product-id="${productId}">+</button>
                </div>
            `;
        } else {
            container.innerHTML = `<button class="btn btn-primary w-100 add-to-cart" data-product-id="${productId}">Add to Cart</button>`;
        }
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