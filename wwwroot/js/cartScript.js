document.addEventListener("DOMContentLoaded", function () {
    const quantitySelectors = document.querySelectorAll('.quantity-selector');

    quantitySelectors.forEach(function (selector) {
        const productId = selector.querySelector('input[name="quantity"]').getAttribute('id').split('_')[1];
        const decreaseBtn = selector.querySelector('button[data-action="decrease"]');
        const increaseBtn = selector.querySelector('button[data-action="increase"]');
        const quantityInput = selector.querySelector('input[name="quantity"]');

        decreaseBtn.addEventListener('click', function () {
            if (quantityInput.value > 1) {
                quantityInput.value--;
            }
        });

        increaseBtn.addEventListener('click', function () {
            quantityInput.value++;
        });

        quantityInput.addEventListener('change', function () {
            if (quantityInput.value < 1) {
                quantityInput.value = 1;
            }
        });
    });
});