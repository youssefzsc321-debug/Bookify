var updatedRow;
function showSuccessMessage(message = (updatedRow === undefined ? 'Saved successfully!' : 'Modified successfully!')) {
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

function OnModalSuccess(row) {
    showSuccessMessage();
    var modal = $('#Modal');
    modal.modal('hide');

    var table = $('.table').DataTable();
    var newRow = $(row);

    if (updatedRow !== undefined) {
        table.row(updatedRow).remove();
        table.row.add(newRow).draw(false).node();
        updatedRow = undefined;
    }
    else {
        table.row.add(newRow).draw(false).node();
    }
    KTMenu.init();
    KTMenu.initHandlers();

}


function OnModalBegin() {
    $('#Modal').find('button[type="submit"]').attr('disabled', 'disabled').attr('data-kt-indicator', 'on');


}

function OnModalComplete() {
    $('#Modal').find('button[type="submit"]').removeAttr('disabled').removeAttr('data-kt-indicator');

}

$(document).ready(function () {

    //Disable submit button on any while processing
    if ($.validator) {
        $.validator.setDefaults({
            ignore: function (index, element) {
                return $(element).is(':hidden') && !$(element).hasClass('select2-hidden-accessible');
            }
        });
    }

    $('form').on('submit', function (e) {
        var $form = $(this);

        if ($form.valid && !$form.valid()) {
            return;
        }

        var formId = $form.attr('id');
        var $submitButton = $form.find('button[type="submit"]');

        if ($submitButton.length === 0 && formId) {
            $submitButton = $('button[type="submit"][form="' + formId + '"]');
        }

        if ($submitButton.length > 0) {
            $submitButton.attr('disabled', 'disabled').attr('data-kt-indicator', 'on');
        }
    });
    //selec2
    $('.js-select2').select2();

    $('.js-select2').on('change', function () {
        $(this).valid();
    });
    if ($.validator) {
        $.validator.setDefaults({
            ignore: [],
        });
    }
    //Datapicker
    $('.js-datepicker').daterangepicker({
        singleDatePicker: true,
        autoApply: true

    });

    //Begin Toggle State
    $(document).on('click', '.js-toggle-status', function () {
        var btn = $(this);
        var url = btn.data('url');
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
                    url: url,
                    type: 'POST',
                    data: {
                        '__RequestVerificationToken': token
                    },
                    success: function (lastUpdatedOn) {


                        var row = btn.closest('tr');
                        var status = row.find('.js-status');

                        if (status.text().trim() === 'Deleted') {

                            status
                                .text('Available')
                                .removeClass('badge-light-danger')
                                .addClass('badge-light-success');

                        } else {

                            status
                                .text('Deleted')
                                .removeClass('badge-light-success')
                                .addClass('badge-light-danger');
                        }

                        row.find('.js-updated-on').html(lastUpdatedOn);

                        row.addClass('animate__animated animate__flash');

                        row.one('animationend webkitAnimationEnd oAnimationEnd', function () {
                            row.removeClass('animate__animated animate__flash');
                        });
                    },
                    error: function () {
                        Swal.fire({
                            icon: 'error',
                            title: 'Oops...',
                            text: 'Something went wrong!'
                        });
                    }
                });
            }
        });
    });
    //End Toggle State
    
   


    //begin renderbutton
    $(document).on('click', '.js-render-model', function () {
        var btn = $(this)
        var modal = $('#Modal')

        if (btn.data('update') !== undefined) {
            updatedRow = btn.parents('tr');
        } else {
            updatedRow = undefined;
        }
        modal.find('#ModalLabel').text(btn.data('title'))

        $.ajax({
            url: btn.data('url'),
            success: function (form) {
                modal.find('.modal-body').html(form)
                $.validator.unobtrusive.parse("#ModalForm");
                var selectElement = modal.find('.js-select2');
                selectElement.select2({
                    dropdownParent: modal
                });

                selectElement.on('change', function () {
                    $(this).valid(); 
                });
            },
            error: function () {
                showErrorMessage()
            }
        })
        modal.modal('show')
    })

    //end renderbutton





    //Begin lock 
    $(document).on('click', '.js-lock-model', function () {
        var btn = $(this);
        var row = btn.closest('tr'); // Target the current table row
        var token = $('input[name="__RequestVerificationToken"]').val();
        // 1. Display the Confirmation Dialog
        Swal.fire({
            title: "Are you sure?",
            text: "Do you really want to unlock this user?",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#198754", // Green for unlock action
            cancelButtonColor: "#6c757d",  // Secondary gray for cancel
            confirmButtonText: "Yes, unlock!",
            cancelButtonText: "Cancel",
            background: "#1e222d",        // Dark theme background for Bookify
            color: "#ffffff"
        }).then((result) => {

            // 2. Check if the user confirmed by clicking "Yes"
            if (result.isConfirmed) {

                // Send AJAX Request to Unlock User
                $.ajax({
                    url: btn.data('url'),
                    method: 'POST',
                    data: {
                        '__RequestVerificationToken': token
                    },
                    success: function (response) {
                        row.find('.js-updated-on').html(response);
                        // A. Trigger CSS fade-out animation by removing the red background class
                        row.removeClass('table-danger-row');

                        // B. Show Success Alert
                        Swal.fire({
                            title: "Unlocked!",
                            text: "User has been unlocked successfully.",
                            icon: "success",
                            timer: 2000,
                            showConfirmButton: false,
                            background: "#1e222d",
                            color: "#ffffff"
                        });

                        // C. Smoothly fade out the unlock button
                        btn.fadeOut();
                    },
                    error: function () {
                        Swal.fire({
                            title: "Error!",
                            text: "Something went wrong while unlocking the user. Please try again.",
                            icon: "error",
                            background: "#1e222d",
                            color: "#ffffff"
                        });
                    }
                });

            }
        });
    });

});
//end lock


//begin datatables
var table = $('#table-js').DataTable({
    info: false,
    searchDelay: 0,
    order: [],
    pageLength: 10,

    drawCallback: function () {
        KTMenu.createInstances();
    },

    dom:
        "<'row'<'col-sm-12'tr>>" +
        "<'row'<'col-sm-12 col-md-5 d-flex align-items-center justify-content-center justify-content-md-start'l>" +
        "<'col-sm-12 col-md-7 d-flex align-items-center justify-content-center justify-content-md-end'p>>",

    buttons: [
        {
            extend: 'copyHtml5',
            className: 'd-none',
            exportOptions: {
                columns: ':not(.js-no-export)'
            }
        },
        {
            extend: 'excelHtml5',
            className: 'd-none',
            exportOptions: {
                columns: ':not(.js-no-export)'
            }
        },
        {
            extend: 'csvHtml5',
            className: 'd-none',
            exportOptions: {
                columns: ':not(.js-no-export)'
            }
        },
        {
            extend: 'pdfHtml5',
            className: 'd-none',
            exportOptions: {
                columns: ':not(.js-no-export)'
            }
        }
    ]
});

$('[data-kt-filter="search"]').on('input', function () {
    table.column(0).search($(this).val()).draw();
});

const exportButtons = document.querySelectorAll('#kt_datatable_example_export_menu [data-kt-export]');

exportButtons.forEach(exportButton => {
    exportButton.addEventListener('click', function (e) {
        e.preventDefault();

        const exportValue = this.getAttribute('data-kt-export');
        table.button('.buttons-' + exportValue).trigger();
    });
});

//end datatables