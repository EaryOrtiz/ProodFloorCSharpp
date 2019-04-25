$(function () {
            $("#JobTypeMain").change(function () {
                var JobTypeMain = $("#JobTypeMain").val();
                var JobTypeAdd = $('#JobTypeAdd');
                JobTypeAdd.empty();
                if (JobTypeMain != null && JobTypeMain != '') {
                    $.ajax({
                        type: 'GET',
                        url: '/Job/GetJobTypeAdd',
                        contentType: "applications/json",
                        data: {
                            JobTypeMain: JobTypeMain
                        },
                        success: function (data) {
                            $("#JobTypeAdd").prop("disabled", false);
                            $.each(data, function (idx, item) {
                                JobTypeAdd.append('<option value="' + item.text + '">' + item.value + '</option>');
                            });
                        },
                        error: function (exc) {
                            alert("error");
                        }
                    });
                }
            });
        });