
$(document).ready(function ()
{ 
    var handle = $("#custom-handle");
    var input = $("#custom-input");
    var videoinput = $("#custom-video-input");
    if (videoinput.val()) {
        var inp_val = videoinput.val()
    }
    else {
        inp_val = 5.0
    }
    
    $("#slider").slider({
        value: inp_val,
        min: 0.1,
        max: 10,
        step: 0.1,
        create: function () {
            handle.text($(this).slider("value"));
            input.val($(this).slider("value"));
            //videoinput.val($(this).slider("value"));
        },
        slide: function (event, ui) {
            handle.text(ui.value);
            input.val(ui.value);
            videoinput.val(ui.value);
        }
    });
});

