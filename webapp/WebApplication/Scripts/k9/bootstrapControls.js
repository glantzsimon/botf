function bootstrapControls(config) {

    function initBootstrapDateTimePickers() {
        $("div.dateonly").datetimepicker({
            format: "L"
        });

        $("div.datetime").datetimepicker({
            format: "L"
        });

        $("div.dateonly, div.datetime").on("dp.change", function (e) {
            var linkId = e.target.firstElementChild.id;
            var formattedDate = e.date.format("YYYY-MM-DD");
            $("input[linkid=" + linkId + "]").val(formattedDate);
        });
    }

    function initBootstrapSelect() {
        $(".selectpicker").selectpicker({
            size: 8
        });
        $("select").each(function () {
            this.getSelectedText = function () {
                return $(this).parent().find("li[data-original-index=" + this.selectedIndex + "] span.text").html();
            };
        });
    }

    function initTextScroller() {
        $(".scroller-container").vTicker({
            speed: 500,
            pause: 3000,
            showItems: 3,
            animation: 'fade',
            mousePause: false,
            height: 0,
            direction: 'up'
        });
    }

    function initImagePreviews() {
        $(".link-image-preview").each(function() {
            $.fn.loadLinkImagePreview($(this));
        });
    }

    function initDateTimeValidation() {
        $.validator.methods.date = function (value, element) {
            return this.optional(element) || moment(value, config.dateFormat, true).isValid() || moment(value, "YYYY-MM-DD", true).isValid();
        }
    }

    function initToolTips() {
        $('[data-toggle="tooltip"]').tooltip();
    }

    function selectFirstFormInput() {
        $("form").find("input[type=text], textarea, select").filter(":not(#main-search):visible:first").focus();
    }

    var init = function () {
        initBootstrapDateTimePickers();
        initBootstrapSelect();
        initDateTimeValidation();
        initToolTips();
        initTextScroller();
        initImagePreviews();
    };

    return {
        init: init,
        selectFirstFormInput: selectFirstFormInput
    };

}