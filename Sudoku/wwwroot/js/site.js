var seconds = 0, minutes = 0;
var timer;

$(function () {
    var slider = $("#slider");
    var size = $("#size");
    var sizes = [4, 6, 8, 9, 10, 12, 14, 15, 16];
    var modeValue = $("input[name='mode']:checked").val();

    // Set the max and default value for the slider
    slider.attr('max', sizes.length - 1);
    slider.val(3);
    size.text("Size: 9x9");

    // Update the size label when the slider changes
    slider.on('input', function () {
        size.text("Size: " + sizes[this.value] + "x" + sizes[this.value]);
    });

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
                size: size.text().substring(5, size.text().length).split('x')[0],
                mode: modeValue
            },
            success: function(result) {
                $("#body").html(result);
                resetTimer();
                if (modeValue === "generate") {
                    $("#submit").attr("onclick", "submitSudoku()");
                    startTimer();
                } else {
                    $("#submit").attr("onclick", "solveSudoku()");
                }
            },
            error: function() {
                alert("error");
            }
        });
    });
});

function solveSudoku() {
    $.ajax({
        url: "/Home/SolveSudoku",
        type: "POST",
        success: function (result) {
            $("#body").html(result);
        },
        error: function () {
            alert("error");
        }
    });
}

function startTimer() {
    if (!timer) {
        timer = setInterval(updateTimer, 1000);
    }
    $("#playButton").attr("hidden", true);
    $("#pauseButton").removeAttr("hidden");
}

function updateTimer() {
    $("#time").text(minutes + ":" + (seconds < 10 ? "0" + seconds : seconds));
    seconds++;
    if (seconds === 60) {
        seconds = 0;
        minutes++;
    }
}

function pauseTimer() {
    clearInterval(timer);
    timer = false;
    $("#pauseButton").attr("hidden", true);
    $("#playButton").removeAttr("hidden");
}

function resetTimer() {
    seconds = 0;
    minutes = 0;
}