document.addEventListener("DOMContentLoaded", function () {
    document.querySelector("form").addEventListener("submit", function () {
        document.querySelectorAll(".hidden input").forEach(input => {
            input.classList.remove("hidden");
        });
    });
});

document.addEventListener("DOMContentLoaded", function () {
    const toggleButton = document.getElementById("part1Button");
    const section = document.getElementById("part1");
    const hiddenDivs = section.querySelectorAll(".hidden");
    toggleButton.addEventListener("click", function (event) {
        event.preventDefault();
        const isHidden = hiddenDivs[0].classList.contains("hidden");
        hiddenDivs.forEach(div => {
            if (isHidden) {
                div.classList.remove("hidden");
                div.classList.add("flex");
            } else {
                div.classList.add("hidden");
                div.classList.remove("flex");
            }
        });
        toggleButton.textContent = isHidden ? "−" : "+";
    });
});

document.addEventListener("DOMContentLoaded", function () {
    const toggleButton = document.getElementById("part2Button");
    const section = document.getElementById("part2");
    const hiddenDivs = section.querySelectorAll(".hidden");
    toggleButton.addEventListener("click", function (event) {
        event.preventDefault();
        const isHidden = hiddenDivs[0].classList.contains("hidden");
        hiddenDivs.forEach(div => {
            if (isHidden) {
                div.classList.remove("hidden");
                div.classList.add("flex");
            } else {
                div.classList.add("hidden");
                div.classList.remove("flex");
            }
        });
        toggleButton.textContent = isHidden ? "−" : "+";
    });
});

document.addEventListener("DOMContentLoaded", function () {
    const toggleButton = document.getElementById("part3Button");
    const section = document.getElementById("part3");
    const hiddenDivs = section.querySelectorAll(".hidden");
    toggleButton.addEventListener("click", function (event) {
        event.preventDefault();
        const isHidden = hiddenDivs[0].classList.contains("hidden");
        hiddenDivs.forEach(section => {
            if (isHidden) {
                section.classList.remove("hidden");
                section.classList.add("flex");
            } else {
                section.classList.add("hidden");
                section.classList.remove("flex");
            }
        });
        toggleButton.textContent = isHidden ? "−" : "+";
    });
});

document.addEventListener("DOMContentLoaded", function () {
    const toggleButton = document.getElementById("part4Button");
    const section = document.getElementById("part4");
    const hiddenDivs = section.querySelectorAll(".hidden");
    toggleButton.addEventListener("click", function (event) {
        event.preventDefault();
        const isHidden = hiddenDivs[0].classList.contains("hidden");
        hiddenDivs.forEach(section => {
            if (isHidden) {
                section.classList.remove("hidden");
                section.classList.add("flex");
            } else {
                section.classList.add("hidden");
                section.classList.remove("flex");
            }
        });
        toggleButton.textContent = isHidden ? "−" : "+";
    });
});