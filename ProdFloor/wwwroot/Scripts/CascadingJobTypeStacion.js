$(function () {
    $("#JobTypeID").change(function () {
                var JobTypeID = $("#JobTypeID").val();
                var Station = $('#Station');
                Station.empty();
                if (JobTypeID != null && JobTypeID != '') {
                    $.ajax({
                        type: 'GET',
                        url: '/Item/GetStation',
                        contentType: "applications/json",
                        data: {
                            JobTypeID: JobTypeID
                        },
                        success: function (data) {
                            $("#Station").prop("disabled", false);
                            $.each(data, function (idx, item) {
                                Station.append('<option value="' + item.text + '">' + item.value + '</option>');
                            });
                        },
                        error: function (exc) {
                            alert("error");
                        }
                    });
                }
    });
});