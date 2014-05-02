function likeMimry(mimryID) {
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
            if (data == 'success') {
                $this.attr('class', 'ml-liked');
                $this.attr('title', 'Liked');
            }
            else {
                window.location = data;
            }
        }
    });
}

function voteMim(mimryID, vote) {
    if ($(this).hasClass('ml-liked')) {
        return;
    }
    $this = $(this);
    $.ajax({
        url: '/Mims/Vote/' + mimryID + '?vote=' + vote,
        type: "POST",
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data == 'success') {
                $this.attr('class', 'ml-liked');
                $this.siblings('.ml-liked').attr('class', 'ml');
            }
            else {
                window.location = data;
            }
        }
    });
}