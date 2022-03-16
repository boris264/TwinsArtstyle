document.addEventListener("click", e => {
    const isDropDownButton = e.target.matches("[dropdown-button");

    if (!isDropDownButton && e.target.closest("[dropdown-menu]") != null) return;

    let dropdown;
    if (isDropDownButton) {
        dropdown = e.target.closest("[dropdown-menu]");
        dropdown.classList.toggle("active");
    }

    document.querySelectorAll("[dropdown-menu].active").forEach(item => {
        if (item === dropdown) return;
        item.classList.remove("active");
    })
})