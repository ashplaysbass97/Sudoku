$(function() {
    var modeValue = $("input[name='mode']:checked").val();
    $("#size").val(9);

    // Enable or disable the difficulty select depending on the selected mode
    $("input[name='mode']").click(function() {
        modeValue = $("input[name='mode']:checked").val();
        $("#difficulty").prop("disabled", modeValue === "solve" ? true : false);
    });

    // New Sudoku button event listener
    $("#newSudoku").submit(function(e) {
        e.preventDefault();

        $.ajax({
            url: "/Home/NewSudoku",
            type: "POST",
            data: {
                difficulty: $("#difficulty").val(),
                size: $("#size").val(),
                mode: modeValue
            },
            success: function(result) {
                $("#grid").html(result);
            },
            error: function() {
                alert("error");
            }
        });
    });
});