$(document).ready(function () {
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

    $("#CompanyId").change(function () {
        $("#ProjectId").empty();
        $("#ProjectId").append('<option value="0">[Select a Project...]</option>');
        $.ajax({
            type: 'POST',
            url: UrlProject,
            dataType: 'json',
            data: { companyId: $("#CompanyId").val() },
            success: function (data) {
                $.each(data,
                    function (i, data) {
                        $("#ProjectId").append('<option value="' + data.ProjectId + '">' + data.Name + '</option>');
                    });
            },
            error: function (ex) {
                alert('Failed to retrieve projects.' + ex);
            }
        });
        return false;
    });
});
