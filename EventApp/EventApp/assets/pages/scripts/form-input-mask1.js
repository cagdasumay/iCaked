var FormInputMask = function () {
    
    var handleInputMasks = function () {

        $("#mask_tarih").inputmask("d/m/y h:s", {
            "placeholder": "dd/MM/yyyy hh:mm"
        }); //multi-char placeholder
        
    }

    var handleIPAddressInput = function () {
        $('#input_ipv4').ipAddress();
        $('#input_ipv6').ipAddress({
            v: 6
        });
    }

    return {
        //main function to initiate the module
        init: function () {
            handleInputMasks();
            handleIPAddressInput();
        }
    };

}();

if (App.isAngularJsApp() === false) { 
    jQuery(document).ready(function() {
        FormInputMask.init(); // init metronic core componets
    });
}