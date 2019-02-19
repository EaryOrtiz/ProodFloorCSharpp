$(function () {
    
            $("#CountryID").change(function () {
                var CountryID = $("#CountryID").val();
                var StateID = $('#StateID');
                StateID.empty();
                if (CountryID != null && CountryID != '') {
                    $.ajax({
                        type: 'GET',
                        url: '/Job/GetJobState',
                        contentType: "applications/json",
                        data: {
                            CountryID: CountryID
                        },
                        success: function (data) {
                            $("#StateID").prop("disabled", false);
                            StateID.append('<option value="">' + " --- Please Select a State--- " + '</option>');
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
            $("#StateID").change(function () {
                var StateID = $("#StateID").val();
                var CityID = $('#CityID');
                CityID.empty();
                if (StateID != null && StateID != '') {
                    $.ajax({
                        type: 'GET',
                        url: '/Job/GetJobCity',
                        contentType: "applications/json",
                        data: {
                            StateID: StateID
                        },
                        success: function (data) {
                            $("#CityID").prop("disabled", false);
                            CityID.append('<option value="">' + " --- Please Select a City--- " + '</option>');
                            $.each(data, function (idx, City) {
                                CityID.append('<option value="' + City.value + '">' + City.text + '</option>');
                            });
                        },
                        error: function (exc) {
                            alert("error");
                        }
                    });
                }
            });
        });