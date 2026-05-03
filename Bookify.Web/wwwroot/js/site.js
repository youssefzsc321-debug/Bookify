var updatedRow;
function showSuccessMessage(message = (updatedRow === undefined ? 'Saved successfully!' :'Modified successfully!')) {
    Swal.fire({
        icon: 'success',
        title: 'Success',
        text: message,
        customClass: {
            confirmButton: "btn btn-outline btn-outline-dashed btn-outline-primary btn-active-light-primary"
        }
    });
}

function showErrorMessage(message = 'Something went wrong!') {
    Swal.fire({
        icon: 'error',
        title: 'Oops...',
        text: message,
        customClass: {
            confirmButton: "btn btn-outline btn-outline-dashed btn-outline-primary btn-active-light-primary"
        }
    });
}

function OnModalSuccess(item) {
    showSuccessMessage();
    var modal = $('#Modal');
    modal.modal('hide');
    if (updatedRow === undefined) {

        $('tbody').append(item); 
    }
    else {
        $(updatedRow).replaceWith(item);
        updatedRow = undefined;
    }
    
}

$(document).ready(function () {


    $(document).on('click','.js-render-model', function () {
        
        var btn=$(this) 
        var modal = $('#Modal') 

        if (btn.data('update') !== undefined) 
        {
            updatedRow = btn.parents('tr');
        }
        else {
            updatedRow = undefined;
        }
        modal.find('#ModalLabel').text(btn.data('title'))

            $.ajax({
                url: btn.data('url'),
                success: function (form) { 

                    modal.find('.modal-body').html(form)  
                    $.validator.unobtrusive.parse("#ModalForm");
                },
                error: function () {
                    showErrorMessage()
                }
            })
        modal.modal('show')
    })
    

    

});
