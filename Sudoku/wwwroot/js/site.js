var seconds = 0, minutes = 0;
var timer;

$(function () {
    var slider = $("#slider");
    var size = $("#size");
    var sizes = [4, 6, 8, 9, 10, 12, 14, 15, 16];
    var modeValue = $("input[name='mode']:checked").val();

    // Set the max and default value for the slider
    slider.attr("max", sizes.length - 1);
    slider.val(3);
    size.text("Size: 9x9");

    // Update the size label when the slider changes
    slider.on("input", function () {
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
                "difficulty": $("#difficulty").val(),
                "size": size.text().substring(5, size.text().length).split("x")[0],
                "mode": modeValue
            },
            success: function(result) {
                $("#body").html(result);
                resetTimer();
                if (modeValue === "generate") {
                    $("#submitButton").attr("onclick", "submitSudoku()");
                    startTimer();
                } else {
                    $("#submitButton").attr("onclick", "solveSudoku()");
                }
                setCellSize();
                toggleButtons(modeValue);
                eventListeners();
            },
            error: function() {
                alert("error");
            }
        });
    });

    setCellSize();
    window.addEventListener("resize", setCellSize);
});

function solveSudoku() {
    $.ajax({
        url: "/Home/SolveSudoku",
        type: "POST",
        data: {
            "sudoku": getGrid()
        },
        success: function (result) {
            $("#body").html(result);
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function getGrid() {
    var cells = $("#grid :input");
    var size = cells.length;

    var grid = new Array(size);
    for (var i = 0; i < size; i++) {
        grid[i] = cells[i].value;
    }
    return grid;
}

function setCellSize() {
    window.requestAnimationFrame(() => {
        var width = document.querySelector(".cell").clientWidth;
        document.querySelectorAll(".cell").forEach((tile) => {
            if (tile.clientHeight !== width) {
                tile.style.height = width + "px";
                tile.style.fontSize = width * 0.75 + "px";
                tile.style.lineHeight = width + "px";
            }
        });
    });
};

function eventListeners() {
    // Add event listeners for the cells
    $(".cell").each(function() {
        $(this).on({
            mouseenter: function () {
                if (!$(this).hasClass("selected")) {
                    if ($(this).hasClass("invalid")) {
                        $(this).css("background-color", "#ffb4a9");
                    } else if ($(this).hasClass("highlighted")) {
                        $(this).css("background-color", "#bbb");
                    } else {
                        $(this).css("background-color", "#e1e1e1");
                    }
                }
            },
            mouseleave: function () {
                $(this).css("background-color", "");
            },
            click: function() {
                cellHighlighting(this);
            }
        });
    });

    // Add event listeners for the keypad
    $("[id^='keypadButton']").each(function () {
        $(this).click(function() {
            if ($(".selected").length > 0) {
                var selectedCell = $(".selected")[0];
                if ($(selectedCell).data("editable") === "True") {
                    $(selectedCell).text($(this).text());
                    cellHighlighting(selectedCell);
                    invalidCellsCheck();
                }
            }
        });
    });

    // Add an event listener for the erase button
    $("#eraseButton").click(function () {
        if ($(".selected").length > 0) {
            var selectedCell = $(".selected")[0];
            if ($(selectedCell).data("editable") === "True") {
                $(selectedCell).text("");
                cellHighlighting(selectedCell);
                invalidCellsCheck();
            }
        }
    });
}

function cellHighlighting(selectedCell) {
    var invalidRegion = false;
    var invalidColumn = false;
    var invalidRow = false;

    // Check whether the region, column, or row of the selected cell is invalid
    if ($(selectedCell).text() !== "") {
        $(".cell").each(function () {
            if ($(this).attr("id") === $(selectedCell).attr("id") || $(this).text() !== $(selectedCell).text()) return true;
            if ($(this).data("region") === $(selectedCell).data("region")) {
                invalidRegion = true;
            } else if ($(this).data("x") === $(selectedCell).data("x")) {
                invalidColumn = true;
            } else if ($(this).data("y") === $(selectedCell).data("y")) {
                invalidRow = true;
            }
        });
    }

    $(".cell").each(function () {
        // Remove existing background-color classes
        $(this).css("background-color", "");
        $(this).removeClass("highlighted selected invalid");

        // Add appropriate classes to the selected cell
        if ($(this).attr("id") === $(selectedCell).attr("id")) {
            $(this).addClass("selected");
            if ($(selectedCell).text() === "") return true;
            if (invalidRegion || invalidColumn || invalidRow) {
                $(this).addClass("invalid");
            }

        // Add appropriate classes to cells in the same region, column, or row as the selected cell
        } else if ($(this).data("region") === $(selectedCell).data("region") ||
            $(this).data("x") === $(selectedCell).data("x") ||
            $(this).data("y") === $(selectedCell).data("y")) {
            $(this).addClass("highlighted");
            if ($(selectedCell).text() === "") return true;
            if ((invalidRegion && $(this).data("region") === $(selectedCell).data("region")) ||
                (invalidColumn && $(this).data("x") === $(selectedCell).data("x")) ||
                (invalidRow && $(this).data("y") === $(selectedCell).data("y"))) {
                $(this).addClass("invalid");
            }
        }
    });
}

function invalidCellsCheck() {
    $(".cell").each(function () {
        var cell = $(this);
        var invalid = false;

        // Remove existing bootstrap text classes
        $(cell).removeClass(function (index, className) {
            return (className.match(/(^|\s)text-\S+/g) || []).join(" ");
        });

        // Check whether the cell is invalid
        $(".cell").each(function () {
            invalid = $(this).attr("id") !== $(cell).attr("id") &&
                $(this).text() === $(cell).text() &&
                ($(this).data("region") === $(cell).data("region") ||
                    $(this).data("x") === $(cell).data("x") ||
                    $(this).data("y") === $(cell).data("y"));
            return !invalid;
        });

        // Add appropriate bootstrap text class
        if (invalid) {
            $(cell).addClass("text-danger");
        } else if ($(cell).data("editable") === "True") {
            (cell).addClass("text-primary");
        }
    });
}

function toggleButtons(mode) {
    $("[id^='keypadButton']").attr("disabled", false);
    $("#notesButton").attr("disabled", mode === "solve");
    $("#hintButton").attr("disabled", mode === "solve");
    $("#undoButton").attr("disabled", false);
    $("#eraseButton").attr("disabled", false);
    $("#submitButton").attr("disabled", false);
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