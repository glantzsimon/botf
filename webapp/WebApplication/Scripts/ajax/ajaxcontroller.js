function ajaxcontroller() {

    function send(url, args, method) {
        var deferred = $.Deferred();

        $.ajax({
            type: method == null ? "GET" : method,
            url: url,
            dataType: "json",
            data: args,
            success: function (data) {
                deferred.resolve({
                    success: true,
                    data
                });
            },
            error: function (response) {
                var responseObject = jQuery.parseJSON(response.responseText);
                deferred.resolve({
                    success: false,
                    message: responseObject.message,
                    stacktrace: responseObject.StackTrace
                });
            }
        });

        return deferred.promise();
    }

    function get(url, args) {
        return send(url, args, "GET");
    }

    function post(url, args) {
        return send(url, args, "POST");
    }

    return {
        get: get,
        post: post
    };

}