var selectedList = [] 
function onAddCopySuccess(copy) {

    var bookId = $(copy).find('.js-copy').data('book-id'); 
    if (selectedList.find(c => c.bookId == bookId)) {
        showErrorMessage("You can not renal more than one edition for the same book.");
        return;
    }
    $('#CopiesForm').prepend(copy);
    $('#CopiesForm').find('#SaveButton').removeClass('d-none');
    reAssignIndex();


    $('#inputOfSearch').val('');
}

function reAssignIndex() {
    selectedList = [];
    var copies = $('.js-copy');

    $.each(copies, function (index, copy) {
        var serial = $(copy).data('book-serial');
        var bookId = $(copy).data('book-id');

        selectedList.push({ serial: serial, bookId: bookId });

        $(copy).attr('name', `SelectedCopies[${index}]`).attr('id', `SelectedCopies_${index}_`);
    });
}



$(document).ready(function () {
    $('#js-search-button').on('click', function (e) {
        e.preventDefault();
        var serial = $('#inputOfSearch').val().trim();


        if (serial == '') {
            showErrorMessage("Please enter a serial number!");
            return;
        }
        if (selectedList.find(c => c.serial == serial)) {
            showErrorMessage("this copy is already added");
            return;
        }


        if ($('.js-copy').length >= maxAllowedRental) {
            showErrorMessage(`you can not retnal more than ${maxAllowedRental}`);
            return;
        }

        $('#search-form')[0].requestSubmit();

    });

    $(document).on('click', '.js-remove-copy', function () {
        var btn = $(this);
        btn.closest('.js-copy-container').remove();
        reAssignIndex();
        if ($('.js-copy').length === 0) {
            $('#SaveButton').addClass('d-none');
        }
    }); 
});
