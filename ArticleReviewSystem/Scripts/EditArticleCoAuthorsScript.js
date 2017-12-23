$(document).ready(function () {
    var max = $("#MaxCoAuthors").val();
    var current = $("#CoAuthorsCounter").val();
    clearAndHideAll();
    updateView();

    function plusFunc() {
        if (current < max) {
            current = Number(current) + 1;
            updateModel();
            updateView();
        }
    }

    function minusFunc() {
        if (current > 0) {
            current = Number(current) - 1;
            clearAndHide(current);
            updateModel();
            updateView();
        }
    }

    function updateModel() {
        $("#MaxCoAuthors").val(max);
        $("#CoAuthorsCounter").val(current);
    }

    function clearAndHideAll() {
        for (var i = 0; i < max; i++) {
            $('#CoAuthorPanel' + i).hide();
        }
    }

    function clearAndHide(n) {
        $('#CoAuthorPanel' + n).find('input:text').val('');
        $('#CoAuthorPanel' + n).hide();
    }

    function updateView() {
        for (var i = 0; i < current; i++) {
            $('#CoAuthorPanel' + i).show();
        }
        $('#CoAuthorsCounter').html(current);
    }

    $("#coAuthPlus").click(function (event) {
        event.preventDefault();
        plusFunc();
    });

    $("#coAuthMinus").click(function (event) {
        event.preventDefault();
        minusFunc();
    });
});