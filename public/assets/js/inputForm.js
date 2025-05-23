let thermalTransportInput;
let waterTransportInput;
let ionicTransportInput;

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
});

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