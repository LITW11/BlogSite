$(() => {
    $("#content, #name").on('keyup', () => {
        const isValid = $("#content").val() && $("#name").val();
        $("#submit").prop('disabled', !isValid);
    });
});