$(document).ready(function ($) {
    $('#SearchPhrase').keypress(function (e) {
        if (e.which == 13) {
            $(".submitButton").click();
            return false;
        }
    });

    $(".submitButton").click(function (event) {
        event.preventDefault();
        $.when(makeAjax()).done(function () {
            resetPages();
            adjustPages();
        });

    });

    $('#SortBy').on('change', function () {
        event.preventDefault();
        $.when(makeAjax()).done(function () {
            resetPages();
            adjustPages();
        });
    })

    $('#ResultsForPage').on('change', function () {
        event.preventDefault();
        $.when(makeAjax()).done(function () {
            resetPages();
            adjustPages();
        });
    })

    $(".myArrow").click(function (event) {
        event.preventDefault();
        var currentPage = parseInt($.trim($(".currentPage").text()), 10);
        var numberOfPages = parseInt($.trim($(".numberOfPages").text()), 10);
        var id = $(this).attr('id');
        if (id == "backArrow") {
            currentPage = currentPage - 1;
        }
        if (id == "backMaxArrow") {
            currentPage = 1;
        }
        if (id == "nextArrow") {
            currentPage = currentPage + 1;
        }
        if (id == "nextMaxArrow") {
            currentPage = numberOfPages;
        }
        $(".currentPage").text(currentPage);
        adjustPages();
        makeAjax();
    });

    var resetPages = function resetPages() {
        var numberOfPages = $("#MaxPages").val();
        $(".currentPage").text(1);
        $(".numberOfPages").text(numberOfPages);
    };

    var adjustPages = function adjustPages() {
        var currentPage = parseInt($.trim($(".currentPage").text()), 10);
        var numberOfPages = parseInt($.trim($(".numberOfPages").text()), 10);
        if (numberOfPages == 0) {
            $("#pageControl").hide();
        } else {
            $("#pageControl").show();
        }
        if (currentPage == 1) {
            $("#backArrow").hide();
            $("#backMaxArrow").hide();
        } else {
            $("#backArrow").show();
            $("#backMaxArrow").show();
        }
        if (currentPage == numberOfPages) {
            $("#nextMaxArrow").hide();
            $("#nextArrow").hide();
        } else {
            $("#nextMaxArrow").show();
            $("#nextArrow").show();
        }

    };

    var makeAjax = function makeAjax() {
        var model = {
            SearchPhrase: $("#SearchPhrase").val(),
            SortBy: $("#SortBy").val(),
            CurrentPage: $(".currentPage").text(),
            NumberOfPages: $(".numberOfPages").text(),
            ResultsForPage: $("#ResultsForPage").val()
        };

        return $.ajax({
            url: '/Admin/ArticlesReviewers',
            type: 'POST',
            data: model,
            datatype: "html",
            success: function (html) {
                $('.Results').replaceWith(html);
            },
            error: function () {
                alert("error");
            }
        });
    };
    adjustPages();
});