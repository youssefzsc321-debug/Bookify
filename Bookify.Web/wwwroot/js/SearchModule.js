$(document).ready(function () {
    var books = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('title'), 
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        remote: {
            url: '/Search/Find?query=%QUERY',
            wildcard: '%QUERY'
        }
    });

    $('#Search').typeahead({
        minLength: 3,
        highlight: true
    }, {
        name: 'book',
        limit: 100,
        display: 'title',
        source: books,
        templates: {
            empty: [
                '<div class="p-3 text-center text-gray-500 fw-bold">',
                'No books were found!',
                '</div>'
            ].join('\n'),
            suggestion: function (data) {
                var title = data.title || data.Title;
                var author = data.author || data.Author || 'Unknown';
                
                return `<div class="py-1">
                            <span class="fw-bold text-white-200">${title}</span><br/>
                            <span class="fs-8 text-gray-500">by ${author}</span>
                        </div>`;
            }
        }
    }).on('typeahead:select', function (e, book) {
        var key = book.key || book.Key;
        if (key) {
            window.location.replace(`/Search/Details?Key=${key}`);
        }
    });
});