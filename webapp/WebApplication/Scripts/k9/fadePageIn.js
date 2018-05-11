function fadePageIn(config)
{

    function doFadeIn()
    {
        var delay = config.isFirstLoad ? 1000 : 200;
        $("div#pageFadeInOverlay").delay(delay).fadeOut(delay, function ()
        {
        });
    }

    function initScrollReveal()
    {
        window.sr = ScrollReveal();
        if (window.matchMedia("(min-width: 992px)").matches)
        {
            var beforeReveal = function (el)
            {
                $(el).trigger("reveal");
            };
            sr.reveal(".scrollFadeUp",
                {
                    duration: 1200,
                    distance: "100px",
                    scale: 1,
                    beforeReveal: function (el)
                    {
                        beforeReveal(el);
                    }
                });
            sr.reveal(".scrollFadeIn",
                {
                    duration: 1000,
                    distance: "4px",
                    scale: 1,
                    beforeReveal: function (el)
                    {
                        beforeReveal(el);
                    }
                });
        }
    }

    var init = function ()
    {
        initScrollReveal();
        doFadeIn();
    };

    return {
        init: init
    };

};