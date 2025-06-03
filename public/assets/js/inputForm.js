let thermalTransportInput;
let waterTransportInput;
let ionicTransportInput;

let edgeElementLengthInput;
let meshTypeInput;
let edgeElementDiv;

let weatherStationInput;
let exposureSeriesInput;

let exposure1Select;
let exposure2Select;

const tableData = {
    t: [],
    h: [],
    i: []
};

document.addEventListener("DOMContentLoaded", () => {
    //INIT Probabilistics inputs
    const waterVaporTransportActivatedCheckbox = document.getElementById("input_form_isWaterVaporTransportActivated");
    const capillarityTransportActivatedCheckbox = document.getElementById("input_form_isCapillarityTransportActivated");
    const ionicTransportActivatedCheckbox = document.getElementById("input_form_isIonicTransportActivated");
    const carbonationActivatedCheckbox = document.getElementById("input_form_isCarbonatationActivated");

    // Corresponding hidden divs
    const vaporWaterTransportDiv = document.getElementById("vaporWaterTransportDiv");
    const liquidWaterTransportDiv = document.getElementById("liquidWaterTransportDiv");
    const chlorideIonicTransportDiv = document.getElementById("chlorideIonicTransportDiv");
    const carbonationDiv = document.getElementById("carbonationDiv");

    //Init inputs
    thermalTransportInput = document.getElementById("input_form_thermalTransport");
    waterTransportInput = document.getElementById("input_form_waterTransport");
    ionicTransportInput = document.getElementById("input_form_IonicTransport");

    edgeElementLengthInput = document.getElementById("input_form_edgeElementLength");
    meshTypeInput = document.getElementById("input_form_meshType");
    edgeElementDiv = document.getElementById("edgeElementDiv")

    weatherStationInput = document.getElementById('input_form_weatherStation');
    exposureSeriesInput = document.getElementById('input_form_exposureSeries');
    exposure1Select = document.getElementById('input_form_exposureFile1');
    exposure2Select = document.getElementById('input_form_exposureFile2');

    const addLineBtn = document.getElementById("addLineBtn");

    const tabs = [
        { buttonId: "inputInfo", sectionId: "inputInfoSection"},
        { buttonId: "programSettings", sectionId: "programSettingsSection" },
        { buttonId: "phAndWaterTransport", sectionId: "phAndWaterTransportSection" },
        { buttonId: "initialConditions", sectionId: "initialConditionsSection" },
        { buttonId: "probabilistic", sectionId: "probabilisticSection" }
    ];

    tabs.forEach(({ buttonId, sectionId }) => {
        const button = document.getElementById(buttonId);
        const section = document.getElementById(sectionId);

        button.addEventListener("click", () => {
            tabs.forEach(({ buttonId, sectionId }) => {
                document.getElementById(sectionId).classList.add("hidden");
                document.getElementById(buttonId).classList.remove("bg-gray-400", "active");
                document.getElementById(buttonId).classList.add("bg-gray-300");
            });

            section.classList.remove("hidden");
            button.classList.add("bg-gray-400", "active");
            button.classList.remove("bg-gray-300");
        });
    });

    function toggleVisibility(checkbox, targetDiv) {
        checkbox.addEventListener("change", () => {
            if (checkbox.checked) {
                targetDiv.classList.remove("hidden");
            } else {
                targetDiv.classList.add("hidden");
            }
        });
    }

    toggleVisibility(waterVaporTransportActivatedCheckbox, vaporWaterTransportDiv);
    toggleVisibility(capillarityTransportActivatedCheckbox, liquidWaterTransportDiv);
    toggleVisibility(ionicTransportActivatedCheckbox, chlorideIonicTransportDiv);
    toggleVisibility(carbonationActivatedCheckbox, carbonationDiv);

    if (waterVaporTransportActivatedCheckbox.checked) vaporWaterTransportDiv.classList.remove("hidden");
    if (capillarityTransportActivatedCheckbox.checked) liquidWaterTransportDiv.classList.remove("hidden");
    if (ionicTransportActivatedCheckbox.checked) chlorideIonicTransportDiv.classList.remove("hidden");
    if (carbonationActivatedCheckbox.checked) carbonationDiv.classList.remove("hidden");

    addInputListenersToCells();
    updateTableData();

    meshTypeInput.addEventListener("change", handleMeshTypeChange);
    handleMeshTypeChange();

    weatherStationInput.addEventListener("change", handleWeatherStationChange);
    exposureSeriesInput.addEventListener("change", handleExposureSeriesChange);
});

//adding form listeners

for(let i =1; i<=4; i++){
    addFormListener(i);
}

function handleWeatherStationChange() {
    const stationId = weatherStationInput.value;

    fetch('/weatherstations/get-series-by-station/' + stationId)
        .then(response => response.json())
        .then(data => {
            const seriesSelect = document.getElementById('input_form_exposureSeries');

            //clear existing options except the first one (option generated by Symfony)
            while (seriesSelect.options.length > 1) {
                seriesSelect.remove(1);
            }

            data.forEach(series => {
                const option = document.createElement('option');
                option.value = series.id;
                option.textContent = series.name;
                seriesSelect.appendChild(option);
            });
        });
}

function handleExposureSeriesChange() {
    const seriesId = exposureSeriesInput.value;

    fetch('/get-exposures-by-series/' + seriesId)
        .then(response => response.json())
        .then(data => {

            //clear existing options except the first one (option generated by Symfony)
            [exposure1Select, exposure2Select].forEach(select => {
                while (select.options.length > 1) {
                    select.remove(1);
                }
            });

            data.forEach(exposure => {
                const option1 = document.createElement('option');
                option1.value = exposure.id;
                option1.textContent = exposure.type;
                exposure1Select.appendChild(option1);

                const option2 = document.createElement('option');
                option2.value = exposure.id;
                option2.textContent = exposure.type;
                exposure2Select.appendChild(option2.cloneNode(true));
            });
        });
}

function handleMeshTypeChange() {
    if(meshTypeInput.value === "2" || meshTypeInput.value === "3") {
        edgeElementDiv.classList.remove("hidden");
    }
    else {
        edgeElementDiv.classList.add("hidden");
    }
}

//Fetchs the data from the table to construct the arrays
function updateTableData() {
    const table = document.getElementById("editableTable");
    const rows = Array.from(table.querySelectorAll("tbody tr"));

    tableData.t = [];
    tableData.h = [];
    tableData.i = [];

    rows.forEach(row => {
        const cells = row.querySelectorAll("td");
        if (cells.length >= 4) {
            const xRaw = cells[0].textContent.trim();
            const x = parseFloat(xRaw);

            if (isNaN(x)) return; //x is needed or nothing will happen

            const tRaw = cells[1].textContent.trim();
            const hRaw = cells[2].textContent.trim();
            const iRaw = cells[3].textContent.trim();

            const tVal = parseFloat(tRaw);
            const hVal = parseFloat(hRaw);
            const iVal = parseFloat(iRaw);

            if (tRaw !== "" && !isNaN(tVal)) {
                tableData.t.push({ x, value: tVal });
            }
            if (hRaw !== "" && !isNaN(hVal)) {
                tableData.h.push({ x, value: hVal });
            }
            if (iRaw !== "" && !isNaN(iVal)) {
                tableData.i.push({ x, value: iVal });
            }
        }
    });

    //here we fill data inside the hidden inputs from symfony
    thermalTransportInput.value = JSON.stringify(tableData.t);
    waterTransportInput.value = JSON.stringify(tableData.h);
    ionicTransportInput.value = JSON.stringify(tableData.i);
}

//Table manipulation logic
function addInputListenersToCells() {
    const cells = document.querySelectorAll("#editableTable tbody td");
    cells.forEach(cell => {
        cell.removeEventListener("input", updateTableData);
        cell.addEventListener("input", updateTableData);
    });
}

//adds a new line to the table before the last row
function ajouterLigne() {
    const table = document.getElementById('editableTable').getElementsByTagName('tbody')[0];
    const newRow = table.insertRow(table.rows.length - 1);
    for (let i = 0; i < 4; i++) {
        const newCell = newRow.insertCell();
        newCell.contentEditable = "true";
        newCell.className = "border border-gray-300 px-4 py-2 focus:outline-blue-500";
    }
    addInputListenersToCells();
    updateTableData();
    console.log("Ligne ajoutée");
}

function addFormListener(id){
    document.addEventListener("DOMContentLoaded", function () {
        const toggleButton = document.getElementById("part"+id+"Button");
        const section = document.getElementById("part"+id);
        const hiddenDivs = section.querySelectorAll(".hidden");
        toggleButton.addEventListener("click", function (event) {
            event.preventDefault();
            const isHidden = hiddenDivs[0].classList.contains("hidden");
            hiddenDivs.forEach(div => {
                if (isHidden) {
                    div.classList.remove("hidden");
                    div.classList.add("flex");
                } else {
                    div.classList.add("hidden");
                    div.classList.remove("flex");
                }
            });
            toggleButton.textContent = isHidden ? "−" : "+";
        });
    });
}