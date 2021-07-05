$(function () {
    
            $("#WiringReason1ID").change(function () {
                var Reason1ID = $("#WiringReason1ID").val();
                var Reason2ID = $('#WiringReason2ID');
                Reason2ID.empty();
                if (Reason1ID != null && Reason1ID != '') {
                    $.ajax({
                        type: 'GET',
                        url: '/WiringReasons/GetReason2',
                        contentType: "applications/json",
                        data: {
                            Reason1ID: Reason1ID
                        },
                        success: function (data) {
                            $("#WiringReason2ID").prop("disabled", false);
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
            }).ready(function () {
                var Reason1ID = $("#WiringReason1ID").val();
                var Reason2ID = $('#WiringReason2ID');
                Reason2ID.empty();
                $("#WiringReason2ID").prop("disabled", true);
                Reason2ID.append('<option value="">' + " --- Please Select a Reason 2--- " + '</option>');
                if (Reason1ID != null && Reason1ID != '') {
                    $.ajax({
                        type: 'GET',
                        url: '/WiringReasons/GetReason2',
                        contentType: "applications/json",
                        data: {
                            Reason1ID: Reason1ID
                        },
                        success: function (data) {
                            $("#WiringReason2ID").prop("disabled", false);
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


            $("#WiringReason2ID").change(function () {
                var Reason2ID = $("#WiringReason2ID").val();
                var Reason3ID = $('#WiringReason3ID');
                Reason3ID.empty();
                if (Reason2ID != null && Reason2ID != '') {
                    $.ajax({
                        type: 'GET',
                        url: '/WiringReasons/GetReason3',
                        contentType: "applications/json",
                        data: {
                            Reason2ID: Reason2ID
                        },
                        success: function (data) {
                            $("#WiringReason3ID").prop("disabled", false);
                            Reason3ID.append('<option value="">' + " --- Please Select a Reason 3 --- " + '</option>');
                            $.each(data, function (idx, reason) {
                                Reason3ID.append('<option value="' + reason.value + '">' + reason.text + '</option>');
                            });
                        },
                        error: function (exc) {
                            alert("error");
                        }
                    });
                }
            }).ready(function () {
                var Reason2ID = $("#WiringReason2ID").val();
                var Reason3ID = $('#WiringReason3ID');
                Reason3ID.empty();
                $("#WiringReason3ID").prop("disabled", true);
                Reason3ID.append('<option value="">' + " --- Please Select a Reason 3 --- " + '</option>');
                if (Reason2ID != null && Reason2ID != '') {
                    $.ajax({
                        type: 'GET',
                        url: '/WiringReasons/GetReason3',
                        contentType: "applications/json",
                        data: {
                            Reason2ID: Reason2ID
                        },
                        success: function (data) {
                            $("#WiringReason3ID").prop("disabled", false);
                            $.each(data, function (idx, reason) {
                                Reason3ID.append('<option value="' + reason.value + '">' + reason.text + '</option>');
                            });
                        },
                        error: function (exc) {
                            alert("error");
                        }
                    });
                }
            });


            $("#WiringReason3ID").change(function () {
                var Reason3ID = $("#WiringReason3ID").val();
                var Reason4ID = $('#WiringReason4ID');
                Reason4ID.empty();
                if (Reason3ID != null && Reason3ID != '') {
                    $.ajax({
                        type: 'GET',
                        url: '/WiringReasons/GetReason4',
                        contentType: "applications/json",
                        data: {
                            Reason3ID: Reason3ID
                        },
                        success: function (data) {
                            $("#WiringReason4ID").prop("disabled", false);
                            Reason4ID.append('<option value="">' + " --- Please Select a Reason 4 --- " + '</option>');
                            $.each(data, function (idx, reason) {
                                Reason4ID.append('<option value="' + reason.value + '">' + reason.text + '</option>');
                            });
                        },
                        error: function (exc) {
                            alert("error");
                        }
                    });
                }
            }).ready(function () {
                var Reason3ID = $("#WiringReason3ID").val();
                var Reason4ID = $('#WiringReason4ID');
                Reason4ID.empty();
                $("#WiringReason4ID").prop("disabled", true);
                Reason4ID.append('<option value="">' + " --- Please Select a Reason 4 --- " + '</option>');
                if (Reason3ID != null && Reason3ID != '') {
                    $.ajax({
                        type: 'GET',
                        url: '/WiringReasons/GetReason4',
                        contentType: "applications/json",
                        data: {
                            Reason3ID: Reason3ID
                        },
                        success: function (data) {
                            $("#WiringReason4ID").prop("disabled", false);
                            $.each(data, function (idx, reason) {
                                Reason4ID.append('<option value="' + reason.value + '">' + reason.text + '</option>');
                            });
                        },
                        error: function (exc) {
                            alert("error");
                        }
                    });
                }
            });

});