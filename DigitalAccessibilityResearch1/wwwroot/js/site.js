
document.addEventListener("DOMContentLoaded", function () {
    const body = document.body;
    const root = document.documentElement;

    const decreaseButton = document.getElementById("decrease-font");
    const increaseButton = document.getElementById("increase-font");
    const contrastButton = document.getElementById("toggle-contrast");
    const spacingButton = document.getElementById("toggle-spacing");
    const resetButton = document.getElementById("reset-accessibility");

    if (!decreaseButton || !increaseButton ||
        !contrastButton || !spacingButton || !resetButton) {
        return;
    }

    let fontSize = 100;

    function updateFontSize() {
        root.style.fontSize = fontSize + "%";
    }

    decreaseButton.addEventListener("click", function () {
        fontSize = Math.max(80, fontSize - 10);
        updateFontSize();
    });

    increaseButton.addEventListener("click", function () {
        fontSize = Math.min(150, fontSize + 10);
        updateFontSize();
    });

    contrastButton.addEventListener("click", function () {
        const enabled = body.classList.toggle("high-contrast");
        contrastButton.setAttribute("aria-pressed", String(enabled));
    });

    spacingButton.addEventListener("click", function () {
        const enabled = body.classList.toggle("increased-spacing");
        spacingButton.setAttribute("aria-pressed", String(enabled));
    });

    resetButton.addEventListener("click", function () {
        fontSize = 100;
        updateFontSize();

        body.classList.remove("high-contrast", "increased-spacing");

        contrastButton.setAttribute("aria-pressed", "false");
        spacingButton.setAttribute("aria-pressed", "false");
    });
});
