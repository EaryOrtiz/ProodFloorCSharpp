$(function () {
            $("#Reason1ID").change(function () {
                var Reason1ID = $("#Reason1ID").val();
                var Reason2ID = $('#Reason2ID');
                Reason2ID.empty();
                if (Reason1ID != null && Reason1ID != '') {
                    $.ajax({
                        type: 'GET',
                        url: '/Reasons/GetReason2',
                        contentType: "applications/json",
                        data: {
                            Reason1ID: Reason1ID
                        },
                        success: function (data) {
                            $("#Reason2ID").prop("disabled", false);
                            Reason2ID.append('<option value="">' + " --- Please Select a Reason 2--- " + '</option>');
                            $.each(data, function (idx, reason) {
                                Reason2ID.append('<option value="' + reason.value + '">' + reason.text + '</option>');
                            });
                        },
                        error: function (exc) {
                            alert("error");
                        }
                    });
                }
            });
        });