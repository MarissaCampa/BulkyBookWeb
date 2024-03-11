var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/user/getall' },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "email", "width": "15%" },
            { "data": "phoneNumber", "width": "10%" },
            { "data": "company.name", "width": "15%" },
            { "data": "", "width": "8%" },

            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                    <a href="/admin/user/upsert?id=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i> Edit</a>
                    <a onClick=Delete('/admin/user/delete/${data}') class="btn btn-danger mx-2"><i class="bi bi-trash3"></i> Delete</a>
                    </div>`
                }
            }
        ]
    });
};
