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