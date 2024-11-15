function scroll_to_element(id) {
    //if (/Chrome/.test(navigator.userAgent) && /Google Inc/.test(navigator.vendor)) {
        var block = document.getElementById(id);
        if (block === null)
            return;
        block.scrollIntoView({ behavior: "smooth" });
    //} else
    //    window.scrollTo(0, 0);
}