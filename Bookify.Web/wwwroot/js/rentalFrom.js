var selectedList = []
var currentCopies = []
var isEdit = false;
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
        var serial = $(copy).val();
        var bookId = $(copy).data('book-id');

        selectedList.push({ serial: serial, bookId: bookId });

        $(copy).attr('name', `SelectedCopies[${index}]`).attr('id', `SelectedCopies_${index}_`);
    });
    
}



$(document).ready(function () {
    if ($('.js-copy').length > 0) {
        isEdit = true;
        reAssignIndex();
        currentCopies = selectedList;
    }


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
        var container = btn.closest('.js-copy-container');
        if (isEdit) {
            btn.toggleClass('btn-light-danger btn-light-success js-remove-copy js-re-add').text("Re-Add");
            container.find('img').css('opacity', '0.5');
            container.find('h4').css('opacity', '0.5').css('text-decoration', 'line-through');
            container.find('.js-copy').toggleClass('js-copy js-removed').removeAttr('name').removeAttr('id');
        }
        else {
            container.remove();
        }
        reAssignIndex();
        if ($('.js-copy').length === 0 || JSON.stringify(currentCopies) === JSON.stringify(selectedList)) {
            $('#SaveButton').addClass('d-none');
        }
        else {
            $('#SaveButton').removeClass('d-none');
        }
    });

    $(document).on('click', '.js-re-add', function () {
        var btn = $(this);
        var container = btn.closest('.js-copy-container');

        btn.toggleClass('btn-light-danger btn-light-success js-remove-copy js-re-add').text("Remove");
        container.find('img').css('opacity', '1');
        container.find('h4').css('opacity', '1').css('text-decoration', 'none');
        container.find('.js-removed').toggleClass('js-copy js-removed');

        reAssignIndex();
        if ($('.js-copy').length === 0 || JSON.stringify(currentCopies) === JSON.stringify(selectedList)) {
            $('#SaveButton').addClass('d-none');
        }
        else {
            $('#SaveButton').removeClass('d-none');
        }
    });
});
