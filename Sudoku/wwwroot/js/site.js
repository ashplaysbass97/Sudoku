var seconds = 0, minutes = 0;
var timer;
var undoStack = new Array();

$(function() {
    var slider = $("#slider");
    var size = $("#size");
    var sizes = [4, 6, 8, 9, 10, 12, 14, 15, 16];
    var modeValue = $("input[name='mode']:checked").val();

    // Set the max and default value for the slider
    slider.attr("max", sizes.length - 1);
    slider.val(3);
    size.text("Size: 9x9");

    // Update the size label when the slider changes
    slider.on("input", function() {
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
                    $("#notesButton").attr("disabled", false);
                    startTimer();
                } else {
                    $("#submitButton").attr("onclick", "solveSudoku()");
                }
                $("#submitButton").attr("disabled", false);
                setCellSize();
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

function submitSudoku() {
    var alerts = [];

    // Add any alerts to the array
    if (isIncomplete()) alerts.push("Incomplete");
    if (isInvalid()) alerts.push("Invalid");

    // Display any alerts
    if (alerts.length === 1)  createAlert("danger", alerts[0]);
    else if (alerts.length > 1) createAlert("danger", alerts.join("<br>"));

    else {
        // Submit Sudoku
    }
}

function solveSudoku() {
    var alerts = [];

    // Add any alerts to the array
    if (isInvalid()) alerts.push("Invalid");

    // Display any alerts
    if (alerts.length === 1) createAlert("danger", alerts[0]);
    else if (alerts.length > 1) createAlert("danger", alerts.join("<br>"));

    else {
        $.ajax({
            url: "/Home/SolveSudoku",
            type: "POST",
            data: {
                "sudoku": getGrid()
            },
            success: function (result) {
                $("#body").html(result);
                setCellSize();
            },
            error: function (error) {
                console.log(error);
            }
        });
    }
    
}

function getGrid() {
    var grid = [];
    $(".cell").each(function() {
        grid.push($(this).text());
    });
    return grid;
}

function setCellSize() {
    var width = $(".cell").width();
    $(".cell").each(function () {
        $(this).css("height", width + "px");
        $(this).css("font-size", width * 0.75 + "px");
        $(this).css("line-height", width + "px");
    });
};

function eventListeners() {
    // Add event listeners for the cells
    $(".cell").each(function() {
        $(this).on({
            mouseenter: function() {
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
            mouseleave: function() {
                $(this).css("background-color", "");
            },
            click: function() {
                cellHighlighting(this);
                toggleButtons();
            }
        });
    });

    // Add event listeners for the keypad
    $("[id^='keypadButton']").each(function() {
        $(this).click(function() {
            updateCell($(this).text());
        });
    });

    // Add an event listener for the hint button
    $("#hintButton").click(function () {
        var selectedCell = $(".selected")[0];
        updateCell($(selectedCell).data("solution"));
    });

    // Add an event listener for the undo button
    $("#undoButton").click(function () {
        undo();
    });

    // Add an event listener for the erase button
    $("#eraseButton").click(function() {
        updateCell("");
    });
}

function updateCell(value) {
    var selectedCell = $(".selected")[0];
    if ($(selectedCell).data("editable") === "True") {
        undoStack.push([selectedCell, $(selectedCell).text()]);
        $(selectedCell).text(value);
        cellHighlighting(selectedCell);
        invalidCellsCheck();
        toggleButtons();
    }
}

function cellHighlighting(selectedCell) {
    var invalidRegion = false;
    var invalidColumn = false;
    var invalidRow = false;

    // Check whether the region, column, or row of the selected cell is invalid
    if ($(selectedCell).text() !== "") {
        $(".cell").each(function() {
            if (this === selectedCell || $(this).text() !== $(selectedCell).text()) return true;
            if ($(this).data("region") === $(selectedCell).data("region")) {
                invalidRegion = true;
            } else if ($(this).data("x") === $(selectedCell).data("x")) {
                invalidColumn = true;
            } else if ($(this).data("y") === $(selectedCell).data("y")) {
                invalidRow = true;
            }
        });
    }

    $(".cell").each(function() {
        // Remove existing background-color classes
        $(this).css("background-color", "");
        $(this).removeClass("highlighted selected invalid");

        // Add appropriate classes to the selected cell
        if (this === selectedCell) {
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
    $(".cell").each(function() {
        var cell = this;
        var invalid = false;

        // Remove existing bootstrap text classes
        $(cell).removeClass(function(index, className) {
            return (className.match(/(^|\s)text-\S+/g) || []).join(" ");
        });

        // Check whether the cell is invalid
        if ($(this).text() !== "") {
            $(".cell").each(function () {
                invalid = this !== cell &&
                    $(this).text() === $(cell).text() &&
                    ($(this).data("region") === $(cell).data("region") ||
                        $(this).data("x") === $(cell).data("x") ||
                        $(this).data("y") === $(cell).data("y"));
                return !invalid;
            });
        }

        // Add appropriate bootstrap text class
        if (invalid) {
            $(cell).addClass("text-danger");
        } else if ($(cell).data("editable") === "True") {
            $(cell).addClass("text-primary");
        }
    });
}

function toggleButtons() {
    var selectedCell = $(".selected")[0];
    $("[id^='keypadButton']").attr("disabled", $(selectedCell).data("editable") === "False");
    $("#hintButton").attr("disabled", $(selectedCell).data("editable") === "False");
    $("#eraseButton").attr("disabled", $(selectedCell).data("editable") === "False");
    $("#undoButton").attr("disabled", undoStack.length === 0);
}

function undo() {
    var lastAction = undoStack.pop();
    $(lastAction[0]).text(lastAction[1]);
    cellHighlighting($(".selected")[0]);
    invalidCellsCheck();
    toggleButtons();
}

function isIncomplete() {
    var incomplete = false;
    $(".cell").each(function () {
        if ($(this).text() === "") {
            incomplete = true;
            return false;
        }
    });
    return incomplete;
}

function isInvalid() {
    var invalid = false;
    $(".cell").each(function () {
        if ($(this).hasClass("text-danger")) {
            invalid = true;
            return false;
        }
    });
    return invalid;
}

function createAlert(type, message) {
    $("#alertContainer").html("<div class='alert alert-dismissible alert-" + type + "'><a href='#' class='close' data-dismiss='alert'>&times;</a>" + message + "</div>");
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