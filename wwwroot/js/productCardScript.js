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
                showNotificationHeart('Produktet er tilføjet til favoritter!');
                setTimeout(() => location.reload(), 2000); // Reload the page after 1 second
            }
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
}

function showNotificationHeart(message) {
    const notification = document.getElementById('notificationHeart');
    notification.innerHTML = `${message} &#10084;`;  // Adds the heart after the message
    notification.classList.add('show');

    // Hide the notification after 3 seconds
    setTimeout(() => {
        notification.classList.remove('show');
    }, 2000);
}


function handleCartClick(productId) {
    fetch('/ShoppingCart/AddToCartBtn', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: new URLSearchParams({
            productId: productId,
            quantity: 1
        })
    })
        .then(response => {
            if (!response.ok) {
                return response.json().then(errorData => {
                    throw new Error('Network response was not ok: ' + JSON.stringify(errorData));
                });
            }
            return response.json();
        })
        .then(data => {
            if (data.success) {
                showNotificationCart('Produktet er tilføjet til kurven!');
                setTimeout(() => location.reload(), 2000); // Reload the page after 1 second
            }
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
}

function showNotificationCart(message) {
    const notification = document.getElementById('notificationCart');
    notification.innerHTML = `${message} <i class="fas fa-shopping-basket" style="color: white;"></i>`;  // Adds the basket icon after the message
    notification.classList.add('show');

    // Hide the notification after 3 seconds
    setTimeout(() => {
        notification.classList.remove('show');
    }, 2000);
}


function updateAntalFilter(value) {
    document.getElementById('antalRangeValue').innerText = value;
    filterProducts();
}

function filterProducts() {
    // Collect filter criteria
    var offersOnly = document.getElementById('offersOnly').checked;
    var inStockOnly = document.getElementById('inStockOnly').checked;
    var selectedBrands = [];
    var selectedColors = [];
    var selectedConditions = [];
    var priceLimit = document.getElementById('priceRange').value;
    var antalLimit = document.getElementById('antalRange').value;

    document.querySelectorAll('#brandFilter input:checked').forEach(function (el) {
        selectedBrands.push(el.value);
    });
    document.querySelectorAll('#colorFilter input:checked').forEach(function (el) {
        selectedColors.push(el.value);
    });
    document.querySelectorAll('#conditionFilter input:checked').forEach(function (el) {
        selectedConditions.push(el.value);
    });

    // Implement the filtering logic based on collected criteria
    // This part should be implemented on the server-side in an actual project

    console.log("Filtering products by:", {
        offersOnly: offersOnly,
        inStockOnly: inStockOnly,
        brands: selectedBrands,
        colors: selectedColors,
        conditions: selectedConditions,
        price: priceLimit,
        antal: antalLimit
    });

    // Add your AJAX or form submission code here to fetch and display filtered products
}


// This function ensures that the price filter is set to the correct initial value.
function setInitialPriceRange() {
    var priceRange = document.getElementById('priceRange');
    var minPrice = parseFloat(priceRange.min);
    var maxPrice = parseFloat(priceRange.max);
    document.getElementById('priceRangeValue').innerText = minPrice;
    priceRange.value = minPrice;
}

window.onload = setInitialPriceRange;


$(document).ready(function () {
    // Attach event handler to the filter form (assume your filter form has id "productFilterForm")
    $('#productFilterForm').on('submit', function (e) {
        e.preventDefault(); // Prevent the default form submission

        $.ajax({
            url: '@Url.Action("Index", "Product")', // The action and controller to send the request to
            type: 'GET', // Use GET or POST depending on your scenario
            data: $(this).serialize(), // Serialize the form data
            success: function (result) {
                // Replace the contents of the product cards container with the new HTML
                $('#productCardsContainer').html($(result).find('#productCardsContainer').html());
            },
            error: function () {
                alert('An error occurred while filtering products.');
            }
        });
    });
});