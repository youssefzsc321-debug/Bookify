


function OnAddSubsccriptionSuccess(rowHtml) {
    $('#Modal').modal('hide');

    if (typeof showSuccessMessage === "function") {
        showSuccessMessage("Subscription added successfully!");
    }

    $('#cardTable').removeClass('d-none');

    $('#SubscriptionsTable tbody').prepend(rowHtml);

    $('#SubscriberStatusBadge')
        .removeClass('badge-light-warning badge-light-danger')
        .addClass('badge-light-primary')
        .text('Active');
    $('#SubscriberStatusBadge')
        .removeClass('badge-light-warning badge-light-danger')
        .addClass('badge-light-primary')
        .text('Active');

    $('#MembershipCard')
        .removeClass('bg-warning bg-danger')
        .addClass('bg-primary');

    $('#MembershipTitle, #MembershipStatusText')
        .removeClass('text-gray-900')
        .addClass('text-white');

    $('#MembershipStatusText').text('Active');

    $('.js-addSubsctiption').addClass('d-none');

    $('#RentalToolbar').removeClass('d-none');
    $('.js-reNew').removeClass('d-none');
}

$(document).ready(function () {
  
    $('.js-reNew').on('click', function (e) {
        e.preventDefault();

        var btn = $(this);
        var key = btn.data('key');
        var token = $('input[name="__RequestVerificationToken"]').val();

        Swal.fire({
            text: "Are you sure you want to renew this subscription?",
            icon: "warning",
            showCancelButton: true,
            buttonsStyling: false,
            confirmButtonText: "Yes, renew it!",
            cancelButtonText: "No, cancel",
            customClass: {
                confirmButton: "btn btn-primary",
                cancelButton: "btn btn-active-light"
            }
        }).then(function (result) {
            if (result.isConfirmed) {

                btn.addClass('disabled').attr('disabled', 'disabled');

                $.ajax({
                    url: '/Subscripers/RenewSubscription?key=' + key,
                    type: 'POST',
                    data: {
                        __RequestVerificationToken: token
                    },
                    success: function (responseRowHtml) {

                        $('#SubscriptionsTable tbody tr').find('.badge-light-primary')
                            .removeClass('badge-light-primary')
                            .addClass('badge-light-danger')
                            .text('Expired');

                        var $newRow = $(responseRowHtml);
                        $('#SubscriptionsTable tbody').append($newRow);

                        var newStatus = $newRow.find('.badge').text().trim();

                        var statusBadge = $('#SubscriberStatusBadge');
                        var membershipCard = $('.card-xl-stretch').filter(function () {
                            return $(this).find('.card-title').text().includes('Membership Status');
                        });
                        var membershipText = membershipCard.find('.fs-2hx');

                        membershipCard.removeClass('bg-primary bg-warning bg-danger');
                        membershipText.removeClass('text-white text-gray-900');
                        statusBadge.removeClass('badge-light-primary badge-light-warning badge-light-danger');

                        if (newStatus === 'Active') {
                            membershipCard.addClass('bg-primary');
                            membershipText.addClass('text-white').text('Active');
                            statusBadge.addClass('badge-light-primary').text('Active');

                            
                            $('#RentalToolbar').removeClass('d-none');

                        } else if (newStatus === 'Not Started' || newStatus === 'Expired') {
                            membershipCard.addClass('bg-warning');
                            membershipText.addClass('text-gray-900').text('Not Active');
                            statusBadge.addClass('badge-light-warning').text('Not Active');

                            $('#RentalToolbar').addClass('d-none');

                        } else if (newStatus === 'Black Listed') {
                            membershipCard.addClass('bg-danger');
                            membershipText.addClass('text-white').text('Black Listed');
                            statusBadge.addClass('badge-light-danger').text('Black Listed');

                            $('#RentalToolbar').addClass('d-none');
                        }

                        Swal.fire({
                            text: "Subscription renewed successfully!",
                            icon: "success",
                            buttonsStyling: false,
                            confirmButtonText: "Ok, got it!",
                            customClass: {
                                confirmButton: "btn btn-primary"
                            }
                        }).then(function () {
                            btn.removeClass('disabled').removeAttr('disabled');
                        });
                    },
                    error: function (xhr) {
                        btn.removeClass('disabled').removeAttr('disabled');

                        Swal.fire({
                            text: xhr.responseText || "Something went wrong! Please try again.",
                            icon: "error",
                            buttonsStyling: false,
                            confirmButtonText: "Ok",
                            customClass: {
                                confirmButton: "btn btn-primary"
                            }
                        });
                    }
                });
            }
        });
    });

    $('.js-cancel-rental').on('click', function (e) {
        e.preventDefault();

        var btn = $(this);
        var rentalId = btn.data('id');
        var token = $('input[name="__RequestVerificationToken"]').val();

        Swal.fire({
            text: "Are you sure you want to cancel this rental?",
            icon: "warning",
            showCancelButton: true,
            buttonsStyling: false,
            confirmButtonText: "Yes, cancel it!",
            cancelButtonText: "No, keep it",
            customClass: {
                confirmButton: "btn btn-danger",
                cancelButton: "btn btn-active-light"
            }
        }).then(function (result) {
            if (result.isConfirmed) {
                btn.addClass('disabled').attr('disabled', 'disabled');

                $.ajax({
                    url: '/Rentals/MarkeDeleted/' + rentalId,
                    type: 'POST',
                    data: {
                        __RequestVerificationToken: token
                    },
                    success: function () {
                        btn.parents('tr').remove();

                        var total = parseInt($('#TotalCopies').data('total-copies')) || 0;
                        var totalofOnRow = parseInt(btn.closest('tr').find('#CopiesOfRentals').data('copies-rental')) || 0;
                        total = total - totalofOnRow

                        $('#TotalCopies').text(total);
                        $('#TotalCopies').data('total-copies', total);
                        if ($('.retnal-table tbody tr').length === 0) {
                            $('.retnal-table').fadeOut(function () {
                                $('#Alert').fadeIn();
                            });

                        }

                       
                        
                    },
                    error: function (xhr) {
                        btn.removeClass('disabled').removeAttr('disabled');

                        Swal.fire({
                            text: xhr.responseText || "Something went wrong! Please try again.",
                            icon: "error",
                            buttonsStyling: false,
                            confirmButtonText: "Ok",
                            customClass: {
                                confirmButton: "btn btn-primary"
                            }
                        });
                    }
                });
            }
        });
    });

    $('.js-addSubsctiption').on('click', function () {
        var btn = $(this);
        var subscriberKey = btn.data('key');
        var modal = $('#Modal');

        modal.find('#ModalLabel').text("Add Subscription");

        $.ajax({
            url: '/Subscripers/AddSubscription/' + subscriberKey,
            type: 'GET',
            success: function (formHtml) {
                modal.find('.modal-body').html(formHtml);

                $.validator.unobtrusive.parse("#ModalForm");

                modal.modal('show');
            },
            error: function () {
                if (typeof showErrorMessage === "function") {
                    showErrorMessage("Failed to load subscription form!");
                }
            }
        });
    });
});