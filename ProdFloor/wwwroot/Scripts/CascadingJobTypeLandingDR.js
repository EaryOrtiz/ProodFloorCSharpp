$(document).ready(function () {
    var JobTypeID = $("#JobTypeNum").val();
    var LandingSystemID = $('#LandingSystemID');
    LandingSystemID.empty();
    console.log(JobTypeID);
    if (JobTypeID != null && JobTypeID != '') {
        $.ajax({
            type: 'GET',
            url: '/Item/GetLandingSystem',
            contentType: "applications/json",
            data: {
                JobTypeID: JobTypeID
            },
            success: function (data) {
                $("#LandingSystemID").prop("disabled", false);
                $.each(data, function (idx, item) {
                    LandingSystemID.append('<option value="' + item.text + '">' + item.value + '</option>');
                });
            },
            error: function (exc) {
                alert("error");
            }
        });
    }
});