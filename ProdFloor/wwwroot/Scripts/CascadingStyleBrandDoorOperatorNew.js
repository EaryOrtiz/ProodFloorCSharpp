$(function () {
    
            $("#Style").ready(function () {
                var Style = $("#Style").val();
                var Brand = $('#Brand');
                var DoorOperatorID = $('#DoorOperatorID').val();
                if(DoorOperatorID == ''){
                    Brand.empty();
                if (Style != null && Style != '') {
                    $.ajax({
                        type: 'GET',
                        url: '/Job/GetBrand',
                        contentType: "applications/json",
                        data: {
                            Style: Style
                        },
                        success: function (data) {
                            $("#Brand").prop("disabled", false);
                            Brand.append('<option value="">' + " --- Please Select a Brand--- " + '</option>');
                            $.each(data, function (idx, brand) {
                                Brand.append('<option value="' + brand.value + '">' + brand.text + '</option>');
                            });
                        },
                        error: function (exc) {
                            alert("error");
                        }
                    });
                }

                }
                
            });
            $("#Brand").change(function () {
                var Brand = $("#Brand").val();
                var DoorOperatorID = $('#DoorOperatorID');
                DoorOperatorID.empty();
                if (Brand != null && Brand != '' ) {
                    $.ajax({
                        type: 'GET',
                        url: '/Job/GetDoorOperatorID',
                        contentType: "applications/json",
                        data: {
                            Brand: Brand
                        },
                        success: function (data) {
                            $("#DoorOperatorID").prop("disabled", false);
                            DoorOperatorID.append('<option value="">' + " ---Please Select a Door Operator--- " + '</option>');
                            $.each(data, function (idx, doorOperatorID) {
                                DoorOperatorID.append('<option value="' + doorOperatorID.value + '">' + doorOperatorID.text + '</option>');
                            });
                        },
                        error: function (exc) {
                            alert("error");
                        }
                    });
                }
            });
        });