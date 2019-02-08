$(function () {
            $("#CountryID").change(function () {
                var CountryID = $("#CountryID").val();
                var StateID = $('#StateID');
                StateID.empty();
                if (CountryID != null && CountryID != '') {
                    $.ajax({
                        type: 'GET',
                        url: '/City/GetJobState',
                        contentType: "applications/json",
                        data: {
                            CountryID: CountryID
                        },
                        success: function (data) {
                            $("#StateID").prop("disabled", false);
                            $.each(data, function (idx, state) {
                                StateID.append('<option value="' + state.value + '">' + state.text + '</option>');
                            });
                        },
                        error: function (exc) {
                            alert("error");
                        }
                    });
                }
            });
        });