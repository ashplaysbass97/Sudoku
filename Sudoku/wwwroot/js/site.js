$(function () {
    var modeValue = $("input[name='mode']:checked").val();

    // Enable or disable the difficulty select depending on the selected mode
    $("input[name='mode']").click(function () {
        modeValue = $("input[name='mode']:checked").val();
        $("#difficulty").prop("disabled", modeValue === "solve" ? true : false);
    });

    // New Sudoku button event listener
    $("#newSudoku").submit(function (e) {
        e.preventDefault();
        var size = $("#size").val().split("x");
        
        $.ajax({
            url: "/Home/NewSudoku",
            type: "POST",
            data: { mode: modeValue },
            success: function (result) {
                alert(result);
            },
            error: function () {
                alert("error");
            }
            });
    });
});


