$('.open-deleteDialog').click(function (e) {
    var articleId = $(this).data('id');
    var link = "/Article/DeleteArticle?articleId=" + articleId;
    $("#deleteLink").attr("href", link);
});