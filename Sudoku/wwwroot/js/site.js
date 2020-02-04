$(function () {
    // Enable or disable the difficulty select depending on the selected mode
    $("input[name='mode']").click(function () {
        var modeValue = $("input[name='mode']:checked").val();
        $("#difficulty").prop("disabled", modeValue === "solve" ? true : false);
    });

    // New Sudoku button event listener
    $("#newSudoku").submit(function (e) {
        e.preventDefault();
        var data = $("#newSudoku :input").serializeArray();
        console.log(data);
    });
});


