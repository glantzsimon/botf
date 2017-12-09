function backgroundImageSwitcher() {

    function preloadImages(imageArray) {
        for (var i = 0; i < imageArray.length; i++) {
            var img = new Image();
            img.src = imageArray[i];
        }
    }

    function init(selector, imageArray) {
        if (imageArray && imageArray.length) {
            preloadImages(imageArray);

            var index = 0;
            var el = $(selector);
            setInterval(function () {
                var src = imageArray[index];
                el.css("background-image", "url(" + src + ")");

                var nextIndex = index + 1 >= imageArray.length ? 0 : index + 1;
                index = nextIndex;
            }, 8000);
        }
    }

    return {
        init: init
    };

}