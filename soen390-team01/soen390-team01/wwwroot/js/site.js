$(function () {
    var placeHolderElement = $('#PlaceHolderHere');
    $('button[data-toggle="ajax-modal"').click(function(event) {
        var url = $(this).data('url');
        var decodedUrl = decodeURIComponent(url);
        $.get(decodedUrl).done(function (data) {
            placeHolderElement.html(data);
            placeHolderElement.find('.modal').modal('show');
        });
    });

    placeHolderElement.on('click', '[data-save="modal"]',
        function (event) {
            var form = $(this).parents('.modal').find('form');
            var actionUrl = form.attr('action');
            var sendData = form.serialize();
            $.post(actionUrl, sendData).done(function (data) {
                var userIdString = sendData.split("&")[0];
                var currentUserId = userIdString.substring(7);
                $('#userRow' + currentUserId).html(data);
                placeHolderElement.find('.modal').modal('hide');
            });
        });
})