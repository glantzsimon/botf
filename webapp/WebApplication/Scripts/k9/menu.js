function menu() {

    function shrinkMenu(logoDiv, els) {
        logoDiv.removeClass("main-logo");
        logoDiv.addClass("small-logo");
        els.each(function () {
            $(this).removeClass("menu-main");
            $(this).addClass("menu-small");
        });
    }

    function growMenu(logoDiv, els) {
        logoDiv.addClass("main-logo");
        logoDiv.removeClass("small-logo");
        els.each(function () {
            $(this).removeClass("menu-small");
            $(this).addClass("menu-main");
        });
    }

    function init() {
        var logoDiv = $("div.default-logo");
        var els = $("#main-logo-container, #main-menu-container, #main-banner, #bs-navbar-collapse-1");
        $(window).scroll(function () {
            var mainMenuTop = $("#main-logo-container").offset().top;
            if (mainMenuTop >= 140) {
                shrinkMenu(logoDiv, els);
            } else {
                growMenu(logoDiv, els);
            }
        });
    }

    return {
        init: init
    };

}