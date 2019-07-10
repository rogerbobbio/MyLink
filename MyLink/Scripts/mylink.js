$(document).ready(function () {
    $("#ProjectId").change(function () {
        $.ajax({
            type: 'POST',
            url: UrlProject,
            dataType: 'json',
            data: { projectId: $("#ProjectId").val() },
            success: function (result) {                
                $.each(result,
                    function (i, result) {
                        //console.log(result.Bank);
                        $('#Account').val(result.Account);
                        $('#Bank').val(result.Bank.Name);
                        $('#PersonCharge').val(result.PersonCharge);
                        $('#InterbankCode').val(result.InterbankCode);
                    });
            },
            error: function (ex) {
                alert('Failed to retrieve project info.' + ex);
            }
        });
        return false;
    });


    $("#DepartmentId").change(function() {
        $("#CityId").empty();
        $("#CityId").append('<option value="0">[Select a city...]</option>');
        $.ajax({
            type: 'POST',
            url: UrlCity,
            dataType: 'json',
            data: { departmentId: $("#DepartmentId").val() },
            success: function(data) {
                $.each(data,
                    function(i, data) {
                        $("#CityId").append('<option value="' + data.CityId + '">' + data.Name + '</option>');
                    });
            },
            error: function(ex) {
                alert('Failed to retrieve cities.' + ex);
            }
        });
        return false;
    });
});
