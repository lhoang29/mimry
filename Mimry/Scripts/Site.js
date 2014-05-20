$(function () {
    voteMim = function (mimryID, vote) {
        if ($(this).hasClass('ml-liked')) {
            vote = 0;
        }
        $this = $(this);
        $.ajax({
            url: '/Mims/Vote/' + mimryID + '?vote=' + vote,
            type: "POST",
            dataType: "html",
            cache: false,
            success: function (data) {
                $this.parent('.ml-pin').html(data);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                if (jqXHR.status == 401) {
                    window.location = jqXHR.responseText;
                }
            }
        });
    }

    $('.ml-pin').on('click', '.ajVoteUp', function () {
        voteMim.call(this, $(this).siblings('[name="MimID"]').val(), 1);
    });
    $('.ml-pin').on('click', '.ajVoteDown', function () {
        voteMim.call(this, $(this).siblings('[name="MimID"]').val(), -1);
    });

    likeMimry = function (mimryID) {
        $this = $(this);
        $.ajax({
            url: '/MimSeqs/Like/' + mimryID,
            type: "POST",
            dataType: "html",
            cache: false,
            success: function (data) {
                $this.parent('.mr-header-actions').html(data);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                if (jqXHR.status == 401) {
                    window.location = jqXHR.responseText;
                }
            }
        });
    }
    $('.mr-header-actions').on('click', '.ajLike', function () {
        likeMimry.call(this, $(this).siblings('[name="MimSeqID"]').val());
    });

    voteMimryComment = function (commentID, vote) {
        if ($(this).hasClass('ml-liked')) {
            vote = 0;
        }
        $this = $(this);
        $.ajax({
            url: '/MimSeqs/VoteComment/' + commentID + '?vote=' + vote,
            type: "POST",
            dataType: "html",
            cache: false,
            success: function (data) {
                $this.parent('.mr-comment-actions').html(data);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                if (jqXHR.status == 401) {
                    window.location = jqXHR.responseText;
                }
            }
        });
    }

    $('.mr-comment-actions').on('click', '.ajComVoteUp', function () {
        voteMimryComment.call(this, $(this).siblings('[name="CommentID"]').val(), 1);
    });
    $('.mr-comment-actions').on('click', '.ajComVoteDown', function () {
        voteMimryComment.call(this, $(this).siblings('[name="CommentID"]').val(), -1);
    });

    $window = $(window);
    $window.load(function () {
        var load = true;
        $window.scroll(function () {
            if (load) {
                if ($window.height() + $window.scrollTop() == $(document).height()) {
                    load = false;
                    $anchorMore = $('.infinite-more-link');
                    var getUrl = $anchorMore.attr('href');
                    if (getUrl) {
                        $.get(getUrl, null,
                            function (data) {
                                $anchorMore.remove();
                                $('#mrMainView').append(data);
                                load = true;
                            },
                        'html');
                    }
                }
            }
        });
    })
});

function clearComment() {
    $('#txtComment').val('');
}