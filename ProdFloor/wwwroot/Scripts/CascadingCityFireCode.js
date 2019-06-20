$(function () {
            $("#CityID").change(function () {
                var CityID = $("#CityID").val();
                var CurrentFireCode = $('#CurrentFireCode');
                CurrentFireCode.empty();
                if (CityID != null && CityID != '') {
                    $.ajax({
                        type: 'GET',
                        url: '/Job/GetCurrentFireCode',
                        contentType: "applications/json",
                        data: {
                            CityID: CityID
                        },
                        success: function (data) {
                            $("#CurrentFireCode").prop("disabled", true);
                            $.each(data, function (idx, item) {
                                CurrentFireCode.append('<option value="' + item.value + '">' + item.text + '</option>');
                            });
                        },
                        error: function (exc) {
                            alert("error");
                        }
                    });
                }
            });
        });