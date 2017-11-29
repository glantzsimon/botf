function revealText() {

    function collapse(logoDiv, els, direction) {

    }

    function expand(logoDiv, els) {

    }

    function init() {
        $(".reveal-text").each(function () {
            var fadeOutDiv = $("<div class='reveal-text-overlay'>&nbsp;</div>");
            $(this).appendChild(fadeOutDiv);
            
        });
        alert("yes");
    }

    return {
        init: init
    };

}