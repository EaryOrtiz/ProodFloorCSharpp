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
                            JobTypeAdd.append('<option value="">' + " --- Please Select a Type of Operation #2--- " + '</option>');
                            $.each(data, function (idx, item) {
                                JobTypeAdd.append('<option value="' + item.value + '">' + item.text + '</option>');
                            });
                        },
                        error: function (exc) {
                            alert("error");
                        }
                    });
                }
            });
        });