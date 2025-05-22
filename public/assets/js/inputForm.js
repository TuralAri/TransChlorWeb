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

});