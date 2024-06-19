$(document).ready(function () {
    $('#searchBox').on('input', function () {
        var searchString = $(this).val();
        clearTimeout($.data(this, 'timer'));
        var wait = setTimeout(function () {
            $.get('/Articles/Index', { searchString: searchString }, function (data) {
                var newDoc = document.open("text/html", "replace");
                newDoc.write(data);
                newDoc.close();
            });
        }, 500);
        $(this).data('timer', wait);
    });
});
