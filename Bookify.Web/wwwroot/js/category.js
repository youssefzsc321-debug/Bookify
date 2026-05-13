var table = $('.table').DataTable({
    "info": false,
    "searchDelay": 0,
    'order': [],
    'pageLength': 10,
    dom: 'Brtip',
    buttons: [
        {
            extend: 'copyHtml5',
            exportOptions: { columns: ':not(.js-no-export)' }
        },
        {
            extend: 'excelHtml5',
            exportOptions: { columns: ':not(.js-no-export)' }
        },
        {
            extend: 'csvHtml5',
            exportOptions: { columns: ':not(.js-no-export)' }
        },
        {
            extend: 'pdfHtml5',
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
        const target = document.querySelector('.dt-buttons .buttons-' + exportValue);

        if (target) {
            target.click();
        }
    });
});