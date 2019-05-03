$(function () {
    $("#JobTypeID").ready(function () {
                var JobTypeID = $("#JobTypeID").val();
                var Stacion = $('#Stacion');
                Stacion.empty();
                if (JobTypeID != null && JobTypeID != '') {
                    $.ajax({
                        type: 'GET',
                        url: '/Item/GetStation',
                        contentType: "applications/json",
                        data: {
                            JobTypeID: JobTypeID
                        },
                        success: function (data) {
                            $("#Stacion").prop("disabled", false);
                            $.each(data, function (idx, item) {
                                Stacion.append('<option value="' + item.text + '">' + item.value + '</option>');
                            });
                        },
                        error: function (exc) {
                            alert("error");
                        }
                    });
                }
            });
        });