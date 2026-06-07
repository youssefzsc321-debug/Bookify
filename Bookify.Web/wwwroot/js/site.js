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

function OnModalBegin() {
    $('#Modal').find('button[type="submit"]').attr('disabled', 'disabled').attr('data-kt-indicator', 'on');


}

function OnModalComplete() {
    $('#Modal').find('button[type="submit"]').removeAttr('disabled').removeAttr('data-kt-indicator');
   
}

$(document).ready(function () {

    //begin datatables
    var table = $('.table').DataTable({
        "info": false,
        "searchDelay": 0,
        'order': [],
        'pageLength': 10,
        
        dom:
            "<'row'<'col-sm-12'tr>>" + 
            "<'row'<'col-sm-12 col-md-5 d-flex align-items-center justify-content-center justify-content-md-start'l>" + 
            "<'col-sm-12 col-md-7 d-flex align-items-center justify-content-center justify-content-md-end'p>>", 
        buttons: [
            {
                extend: 'copyHtml5',
                className: 'd-none',
                exportOptions: { columns: ':not(.js-no-export)' }
            },
            {
                extend: 'excelHtml5',
                className: 'd-none',
                exportOptions: { columns: ':not(.js-no-export)' }
            },
            {
                extend: 'csvHtml5',
                className: 'd-none',
                exportOptions: { columns: ':not(.js-no-export)' }
            },
            {
                extend: 'pdfHtml5',
                className: 'd-none',
                exportOptions: { columns: ':not(.js-no-export)' }
            }
        ]
    });

    $('[data-kt-filter="search"]').on('input', function () {
        table.column(0).search($(this).val()).draw();
    });

    const exportButtons = document.querySelectorAll('#kt_datatable_example_export_menu [data-kt-export]');

    exportButtons.forEach(exportButton => {
        exportButton.addEventListener('click', e => {
            e.preventDefault();

            
            const exportValue = e.target.getAttribute('data-kt-export');
            table.button('.buttons-' + exportValue).trigger();
        });
    });
    //end datatables


    //begin renderbutton
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
    //end renderbutton


    //Begin Toggle State
    $(document).on('click', '.js-toggle-status', function () {
        var btn = $(this);
        var id = btn.data('id');
        var token = $('input[name="__RequestVerificationToken"]').val();

        bootbox.confirm({
            message: 'Are you sure you want to toggle status?',
            buttons: {
                confirm: { label: 'Yes', className: 'btn-success' },
                cancel: { label: 'No', className: 'btn-danger' }
            },
            callback: function (result) {
                if (!result) return;

                $.ajax({
                    url: '/Categories/ToggleStatus/' + id,
                    type: 'POST',
                    data: { '__RequestVerificationToken': token },
                    success: function (lastUpdatedOn) {
                        var row = btn.closest('tr');
                        var status = row.find('.js-status');

                        var isDeleted = status.text().trim() === 'Deleted';
                        var newStatus = isDeleted ? 'Available' : 'Deleted';

                        status.text(newStatus);
                        status.toggleClass('badge-light-success badge-light-danger');

                        row.find('.js-updated-on').html(lastUpdatedOn);
                        row.addClass('animate__animated animate__flash');

                        //const Toast = Swal.mixin({
                        //    toast: true,
                        //    position: 'top-end',
                        //    showConfirmButton: false,
                        //    timer: 2000,
                        //    timerProgressBar: true,
                        //});
                        //Toast.fire({
                        //    icon: 'success',
                        //    title: 'Status updated successfully'
                        //});
                    },
                    error: function (xhr) {
                        Swal.fire({
                            icon: 'error',
                            title: 'Oops...',
                            text: 'Something went wrong!',
                        });
                    }
                });
            }
        });
    });
    //Begin Toggle State

    

    

});
