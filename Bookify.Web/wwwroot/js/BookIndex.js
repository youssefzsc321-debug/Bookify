 $(document).ready(function () {
     var table = $('#BooksTable').DataTable({ 
         serverSide: true, 
         ajax: { 
             url: '/Books/GetBooks', 
             type: 'POST' 
         },
         order: [ 
             [1, 'asc']
         ],
         processing: true, 
         
         language: {
             processing: `
                         <div class="d-flex dt-spinner flex-column align-items-center py-0">
                             <div class="spinner-border text-primary" style="width:1.5rem;height:1.5rem;" role="status"></div>
                             <div class="mt-0 fw-semibold text-gray-700">
                                 Loading...
                             </div>
                         </div>

                     `
            
         },
         stateSave: true,
      
         columnDefs: [{
             targets: [0],
             visible: false, 
             searchable: false 

         }],
         columns: [
             { "data": "id", "name": "Id", className: "d-none" },
             
             {
                 
                 "data": "title", "name": "Title", "render": function (data, type, row) {
                     var imagePath = (row.imageThumbnailUrl === null ? '/Images/image-placeholder.jpg' : row.imageThumbnailUrl);

                     return `<div class="d-flex align-items-center">
                         <div class="symbol symbol-60px h-80px rounded overflow-hidden me-4 shadow-sm">
                             <a href="/Books/Details/${row.id}">
                                 <div class="symbol-label w-60px h-80px bg-light d-flex align-items-center justify-content-center">
                                     <img src="${imagePath}" alt="${data}" class="h-100 w-100" style="object-fit: cover;">
                                 </div>
                             </a>
                         </div>
                         <div class="d-flex flex-column">
                             <a href="/Books/Details/${row.id}" class="text-primary-800 text-hover-primary fw-bold mb-1 fs-6">
                                 ${row.title}
                             </a>
                             <span class="text-gray-400 fs-7">${row.authorsName}</span>
                         </div>
                     </div>`;
                 }
             },
             
             { "data": "publisher", "name": "Publisher" },

             
             {
                 "data": "publishingDate", "name": "PublishingDate", "render": function (data, type, row) {
                     return moment(row.publishingDate).format('ll');
                 }
             },

             { "data": "hall", "name": "Hall" },

             
             { "data": "categories", "name": "Categories", "orderable": false },



             
             {
                 "data": "isAvailableForRental", "name": "IsAvailableForRental", "render": function (data, type, row) {
                     return `<span class="badge badge-light-${(row.isAvailableForRental ? 'success' : 'warning')} js-status">
                                             ${(row.isAvailableForRental ? 'Available' : 'NotAvailable')}
                                    </span>`
                 }
             },


             {
                 "data": "isDeleted", "name": "IsDeleted", "render": function (data, type, row) {
                     return `<span class="badge badge-light-${(row.isDeleted ? 'success' : 'danger')}">
                                             ${(row.isDeleted ? 'Available' : 'Deleted')}
                                    </span>`
                 }
             },


             {
                 "orderable": false,
                 "render": function (data, type, row) {
                     return `
                                  <button type="button" class="btn btn-sm btn-light btn-active-light-primary btn-flex align-items-center" data-kt-menu-trigger="click" data-kt-menu-placement="bottom-end">
                                      Actions
                                      <span class="svg-icon svg-icon-5 m-0 ms-2">
                                          <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                              <path d="M11.4343 12.7344L7.25 8.55005C6.83579 8.13583 6.16421 8.13584 5.75 8.55005C5.33579 8.96426 5.33579 9.63583 5.75 10.05L11.05 15.35C11.4405 15.7405 12.0732 15.7405 12.4637 15.35L17.75 10.05C18.1642 9.63583 18.1642 8.96426 17.75 8.55005C17.3358 8.13584 16.6642 8.13583 16.25 8.55005L12.5657 12.7344C12.2533 13.0468 11.7467 13.0468 11.4343 12.7344Z" fill="currentColor" />
                                          </svg>
                                      </span>
                                  </button>

                                  <div class="menu menu-sub menu-sub-dropdown menu-column menu-rounded menu-gray-800 menu-state-bg-light-primary fw-semibold w-150px py-3" data-kt-menu="true">
                                      <div class="menu-item px-3">
                                          <a href="/Books/Edit/${row.id}" class="menu-link px-3">
                                              <span class="menu-icon"><i class="bi bi-pencil-square"></i></span>
                                              Edit
                                          </a>
                                      </div>
                                      <div class="menu-item px-3">
                                        <a href="javascript:;" class="menu-link px-3 flex-stack js-toggle-status"
                                           data-id="${row.id}"
                                           data-url="/Books/ToggleStatus/${row.id}">
                                            Toggle Status
                                        </a>
                                     </div>`;
                 }
             },



         ],

         drawCallback: function () {
             KTMenu.createInstances();
         },
     });

     $('#kt_filter_search').on('keyup', function () {
         table.search(this.value).draw();
     });



 });