var selectedList = [] 
function onAddCopySuccess(copy) {

    var bookId = $(copy).find('.js-copy').data('book-id'); 
    if (selectedList.find(c => c.bookId == bookId)) {
        showErrorMessage("You can not renal more than one edition for the same book.");
        return;
    }
    $('#CopiesForm').prepend(copy); 
    var copies = $('.js-copy'); 
    selectedList = [] 


    $.each(copies, function (index, copy) {
        var serial = $(copy).val();
        
        var bookId = $(copy).data('book-id');
        selectedList.push({ serial: serial, bookId: bookId }); 

        $(copy).attr('name', `SelectedCopies[${index}]`).attr('id', `SelectedCopies_${index}_`); 

        
    })

    $('#inputOfSearch').val('');




}
$(document).ready(function () {
    $('#js-search-button').on('click', function (e) { 
        e.preventDefault();
        var serial = $('#inputOfSearch').val().trim();
       

        if (serial == '') {
            showErrorMessage("Please enter a serial number!");
            return;
        }
        if (selectedList.find(c => c.serial == serial)){
            showErrorMessage("this copy is already added");
            return;
        }
        
        var maxAllowedRental = $(this).data('max-allowed-rental'); 
        if ($('.js-copy').length >= maxAllowedRental) { 
            showErrorMessage(`you can not retnal more than ${maxAllowedRental}`);
            return;
        }

        $('#search-form').submit(); 

    })
});
