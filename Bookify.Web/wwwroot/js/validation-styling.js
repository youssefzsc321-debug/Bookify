$.validator.setDefaults({
    errorClass: "is-invalid",
    validClass: "is-valid",

    highlight: function (element, errorClass, validClass) {
        $(element).addClass("is-invalid").removeClass("is-valid");
        var elementName = $(element).attr("name");
        $(element.form).find("[data-valmsg-for='" + elementName + "']").addClass("invalid-feedback");
    },

    unhighlight: function (element, errorClass, validClass) {
        $(element).addClass("is-valid").removeClass("is-invalid");
        var elementName = $(element).attr("name");
        $(element.form).find("[data-valmsg-for='" + elementName + "']").removeClass("invalid-feedback");
    },
});