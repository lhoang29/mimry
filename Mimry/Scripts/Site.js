function likeMimry(mimryID) {
    $this = $(this);
    $.ajax({
        url: '/MimSeqs/Like/' + mimryID,
        type: "POST",
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data == 'success') {
                var currentlyLiked = $this.hasClass('ml-liked');
                $this.attr('class', currentlyLiked ? 'ml' : 'ml-liked');
                $this.attr('title', currentlyLiked ? 'Like' : 'Liked');
            }
            else {
                window.location = data;
            }
        }
    });
}

function voteMim(mimryID, vote) {
    if ($(this).hasClass('ml-liked')) {
        vote = 0;
    }
    $this = $(this);
    $.ajax({
        url: '/Mims/Vote/' + mimryID + '?vote=' + vote,
        type: "POST",
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data == 'success') {
                if (vote != 0) {
                    $this.attr('class', 'ml-liked');
                }
                else {
                    $this.attr('class', 'ml');
                }
                $this.siblings('.ml-liked').attr('class', 'ml');
            }
            else {
                window.location = data;
            }
        }
    });
}

function clearComment() {
    $('#txtComment').val('');
}