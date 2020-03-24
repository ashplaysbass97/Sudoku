var mode;
var selectedCell;
var undoStack = new Array();
var seconds = 0, minutes = 0;
var timer;

$(function() {
    var slider = $("#slider");
    var size = $("#size");
    var sizes = [4, 6, 8, 9, 10, 12, 14, 15, 16];

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
        $("#difficulty").prop("disabled", $(this).val() === "solve" ? true : false);
    });

    // New Sudoku button event listener
    $("#newSudoku").submit(function(e) {
        e.preventDefault();
        mode = $("input[name='mode']:checked").val();
        $.ajax({
            url: "/Home/NewSudoku",
            type: "POST",
            data: {
                "difficulty": $("#difficulty").val(),
                "size": size.text().substring(5, size.text().length).split("x")[0],
                "mode": mode
            },
            success: function(result) {
                $("#body").html(result);
                resetTimer();
                undoStack = new Array();
                setCellSize();
                addEventListeners();
                updateButtons();
                if (mode === "generate") startTimer();
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
        $.ajax({
            url: "/Home/SubmitSolution",
            type: "POST",
            data: {
                "sudoku": getGrid()
            },
            success: function (result) {
                $("#body").html(result);
                setCellSize();
                addEventListeners();
                updateButtons();
            },
            error: function (error) {
                console.log(error);
            }
        });
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
                addEventListeners();
                updateButtons();
            },
            error: function (error) {
                console.log(error);
            }
        });
    }
    
}

function setCellSize() {
    var width = $(".cell").width();
    $(".cell").each(function () {
        $(this).css("height", width + "px");
        $(this).css("font-size", width * 0.75 + "px");
        $(this).css("line-height", width + "px");
    });
};

function addEventListeners() {
    // Add event listeners for the cells
    $(".cell").each(function() {
        $(this).on({
            mouseenter: function() {
                if (!$(this).hasClass("selected")) {
                    if ($(this).hasClass("invalid")) $(this).css("background-color", "#ffb4a9");
                    else if ($(this).hasClass("highlighted")) $(this).css("background-color", "#bbb");
                    else $(this).css("background-color", "#e1e1e1");
                }
            },
            mouseleave: function() {
                $(this).css("background-color", "");
            },
            click: function() {
                selectedCell = this;
                highlightCells();
                updateButtons();
            }
        });
    });

    // Add event listeners for the keypad
    $("[id^='keypadButton']").each(function () {
        $(this).click(function () {
            if ($(selectedCell).text() === $(this).text()) updateCell("");
            else updateCell($(this).text());
        });
    });

    // Add an event listener for the hint button
    $("#hintButton").click(function () {
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

    $("#submitButton").attr("onclick", mode === "generate" ? "submitSudoku()" : "solveSudoku()");
}

function updateCell(value) {
    if ($(selectedCell).data("editable") === "True") {
        undoStack.push([$(selectedCell).data("x"), $(selectedCell).data("y"), $(selectedCell).text()]);
        $(selectedCell).text(value);
        highlightCells();
        identifyInvalidCells();
        updateButtons();
    }
}

function undo() {
    var lastAction = undoStack.pop();
    $('.cell[data-x="' + lastAction[0] + '"][data-y="' + lastAction[1] + '"]').text(lastAction[2]);
    highlightCells();
    identifyInvalidCells();
    updateButtons();
}

function highlightCells() {
    var invalidRegion = false;
    var invalidColumn = false;
    var invalidRow = false;

    // Check whether the region, column, or row of the selected cell is invalid
    if ($(selectedCell).text() !== "") {
        $(".cell").each(function() {
            if (this === selectedCell || $(this).text() !== $(selectedCell).text()) return true;
            if ($(this).data("region") === $(selectedCell).data("region")) invalidRegion = true;
            else if ($(this).data("x") === $(selectedCell).data("x")) invalidColumn = true;
            else if ($(this).data("y") === $(selectedCell).data("y")) invalidRow = true;
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
            if (invalidRegion || invalidColumn || invalidRow) $(this).addClass("invalid");

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

function identifyInvalidCells() {
    $(".cell").each(function() {
        var cell = this;
        var editable = $(cell).data("editable") === "True";
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
        if (invalid) $(cell).addClass("text-danger");
        else if (editable) $(cell).addClass("text-primary");
    });
}

function updateButtons() {
    var editable = $(selectedCell).data("editable") === "True";

    // Highlight the corresponding key for the selected cell
    $("[id^='keypadButton']").each(function () {
        if ($(selectedCell).text() === $(this).text()) $(this).addClass("focus");
        else $(this).removeClass("focus");
    });

    // Leave buttons disabled if the Sudoku has been solved
    if ($(".board").data("solved") === "True") return;

    // Enable or disable buttons appropriately
    $("[id^='keypadButton']").attr("disabled", !editable);
    $("#notesButton").attr("disabled", mode === "solve");
    $("#hintButton").attr("disabled", mode === "solve" || !editable);
    $("#eraseButton").attr("disabled", !editable || $(selectedCell).text() === "");
    $("#undoButton").attr("disabled", undoStack.length === 0);
    $("#submitButton").attr("disabled", false);
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

function getGrid() {
    var grid = [];
    $(".cell").each(function () {
        grid.push($(this).text());
    });
    return grid;
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