﻿function likeMimry(mimryID) {
    if ($(this).hasClass('ml-liked')) {
        return;
    }
    $this = $(this);
    $.ajax({
        url: '/MimSeqs/Like/' + mimryID,
        type: "POST",
        dataType: "json",
        cache: false,
        success: function (data) {
            $this.attr('class', 'ml-liked');
            $this.attr('title', 'Liked');
        },
        error: function (request, status, err) {
            if (request.status == 403) {
                var response = $.parseJSON(request.responseText);
                window.location = response.LogOnUrl;
            }
        },
        complete: function () {
        }
    });
}