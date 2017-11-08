$(document).on('click', '#close-preview', function () {
    $('.article-preview').popover('hide');
    // Hover befor close the preview
    $('.article-preview').hover(
        function () {
            $('.article-preview').popover('show');
        },
         function () {
             $('.article-preview').popover('hide');
         }
    );
});

$(function () {
    // Create the close button
    var closebtn = $('<button/>', {
        type: "button",
        text: 'x',
        id: 'close-preview',
        style: 'font-size: initial;',
    });
    closebtn.attr("class", "close pull-right");
    // Clear event
    $('.article-preview-clear').click(function () {
        $('.article-preview').attr("data-content", "").popover('hide');
        $('.article-preview-filename').val("");
        $('.article-preview-clear').hide();
        $('.article-preview-input input:file').val("");
        $(".article-preview-input-title").text("Browse");
    });

    $(".article-preview-input input:file").change(function () {
        var file = this.files[0];
        var reader = new FileReader();
        reader.onload = function (e) {
            $(".article-preview-input-title").text("Change");
            $(".article-preview-clear").show();
            $(".article-preview-filename").val(file.name);
        }
        reader.readAsDataURL(file);
    });
});