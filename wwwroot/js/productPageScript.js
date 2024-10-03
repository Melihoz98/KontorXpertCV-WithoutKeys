function handleHeartClick(productId) {
    fetch('/Favorite/AddToFavorites', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: new URLSearchParams({
            productId: productId
        })
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            if (data.success) {
                alert('Product added to favorites!');
                location.reload();
            }
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
}












document.addEventListener("DOMContentLoaded", function () {
    const mainImage = document.getElementById("mainImage");
    const thumbnailImages = document.querySelectorAll(".thumbnail-image");
    const quantityInput = document.getElementById("quantity");
    const decreaseQuantityBtn = document.getElementById("decreaseQuantity");
    const increaseQuantityBtn = document.getElementById("increaseQuantity");

    // Image switching
    thumbnailImages.forEach(function (thumbnail) {
        thumbnail.addEventListener("click", function () {
            const newImageUrl = thumbnail.getAttribute("src");
            mainImage.setAttribute("src", newImageUrl);
        });
    });

    // Quantity selector
    decreaseQuantityBtn.addEventListener("click", function () {
        if (quantityInput.value > 1) {
            quantityInput.value--;
        }
    });

    increaseQuantityBtn.addEventListener("click", function () {
        quantityInput.value++;
    });

    // Update hidden input for quantity before submitting form
    const addToCartForm = document.querySelector('.add-to-cart form');
    addToCartForm.addEventListener("submit", function () {
        document.getElementById("quantityInput").value = quantityInput.value;
    });
});