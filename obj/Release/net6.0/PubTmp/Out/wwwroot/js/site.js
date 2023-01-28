// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var alertList = document.querySelectorAll('.alert')
alertList.forEach(function (alert) {
  new bootstrap.Alert(alert)
})

var app = document.getElementById('app');

$('#Department').on('change', function () {
    var selectedDepartment = $("#Department").val();
    var EmployeeSelect = $('#Employee');
    EmployeeSelect.empty();
    if (selectedDepartment != null && selectedDepartment != '') {
        $.getJSON('GetEmployees', { departmentName: selectedDepartment }, function (employees) {
            if (employees != null && !jQuery.isEmptyObject(employees)) {
                $.each(employees, function (index, employee) {
                    EmployeeSelect.append($('<option/>', {
                        value: employee.value   ,
                        text: employee.text
                    }));
                });
            };
        });
    }
});


$('#TestType').on('change', function () {
    var selectedType = $("#TestType").val();
    if (selectedType != null && selectedType != '') {
        $.getJSON('GetTestCost', { TypeName: selectedType }, function (CDET) {
            $("#TestCost").val(CDET);
        });
    }
});

$('#CheckType').on('change', function () {
    var selectedType = $("#CheckType").val();
    if (selectedType != null && selectedType != '') {
        $.getJSON('GetCheckCost', { TypeName: selectedType }, function (CDET) {
            $("#CheckCost").val(CDET);
        });
    }
});

$(".id-data").on("click", function () {
    let CID = $(this).children("p").text();
    $("#CheckId").val($(this).children("p").text());

    $.getJSON('Check/GetCheckDetails', { CID: CID }, function (CDET) {
        $("#CheckDetBox").val(CDET);
    });
});

$(".add-test").on("click", function () {
    
    let x = $(this).parent().find("input").val()

    $("#TestId").val(x);

    $("#UploadBtn").trigger("click");

    document.getElementById("UploadBtn").onchange = function() {
        document.getElementById("UploadFileForm").submit();
    };
});
