$(function () {


    $("#Brand").change(function () {
        var Brand = $("#Brand").val();
        var DoorOperatorID = $('#DoorOperatorID');
        DoorOperatorID.empty();
        if (Brand != null && Brand != '') {
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