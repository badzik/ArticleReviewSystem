$(document).ready(function ($) {
    $('#myForm').submit(function (eventObj) {
        var currentPage = parseInt($.trim($(".currentPage").text()), 10);
        var numberOfPages = parseInt($.trim($(".numberOfPages").text()), 10);
        $(this).append('<input type="hidden" name="CurrentPage" value="'+currentPage+'" /> ');
        $(this).append('<input type="hidden" name="NumberOfPages" value="' + numberOfPages + '" /> ');
        return true;
    });

    $(".searchButton").click(function (event) {
        event.preventDefault();
        $.when(makeSearchAjax()).done(function () {
            resetPages();
            adjustPages();
        });
    });

    $("#finalAccept").click(function (event) {
        $('#myForm').submit();
    });

    $('#SortBy').on('change', function () {
        event.preventDefault();
        $.when(makeSearchAjax()).done(function () {
            resetPages();
            adjustPages();
        });
    })

    $('#ResultsForPage').on('change', function () {
        event.preventDefault();
        $.when(makeSearchAjax()).done(function () {
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
        makeSearchAjax();
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


    var makeSearchAjax = function makeSearchAjax() {
        var model = {
            SearchPhrase: $("#SearchPhrase").val(),
            SortBy: $("#SortBy").val(),
            CurrentPage: $(".currentPage").text(),
            NumberOfPages: $(".numberOfPages").text(),
            ResultsForPage: $("#ResultsForPage").val()
        };
        var articleId= getUrlParameter("articleId");

        return $.ajax({
            url: '/Admin/ReviewersSearchAssign',
            type: 'POST',
            data: {ram : model,articleId},
            datatype: "html",
            success: function (html) {
                $('.Results').replaceWith(html);
            },
            error: function () {
                alert("error");
            }
        });
    };

    var getUrlParameter = function getUrlParameter(sParam) {
        var sPageURL = decodeURIComponent(window.location.search.substring(1)),
            sURLVariables = sPageURL.split('&'),
            sParameterName,
            i;

        for (i = 0; i < sURLVariables.length; i++) {
            sParameterName = sURLVariables[i].split('=');

            if (sParameterName[0] === sParam) {
                return sParameterName[1] === undefined ? true : sParameterName[1];
            }
        }
    };

    adjustPages();
});