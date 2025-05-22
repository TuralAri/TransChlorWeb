document.addEventListener("DOMContentLoaded", () => {
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
});