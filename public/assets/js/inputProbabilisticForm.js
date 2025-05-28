let inputMaterials;
let inputProbilisticMaterials;

let transportTypes = {
    "waterVaporTransport": "vaporWaterTransportDiv",
    "capillarityTransport": "liquidWaterTransportDiv",
    "ionicTransport": "chlorideIonicTransportDiv",
    "carbonation": "carbonationDiv"
}

document.addEventListener("DOMContentLoaded", () => {
    // Initialize inputs
    inputMaterials = document.getElementById("input_form_material");
    inputProbilisticMaterials = document.getElementById("selectedMaterials");

    inputMaterials.addEventListener("change", updateSelectedMaterials);
    inputProbilisticMaterials.addEventListener("change", showForm);
});

function updateSelectedMaterials() {
    inputProbilisticMaterials.innerHTML = '<option value="0">--Sélectionner un matériau--</option>';

    for (let key in transportTypes) {
        const div = document.getElementById(transportTypes[key]);
        div.innerHTML = '';
    }

    Array.from(inputMaterials.selectedOptions).forEach(option => {
        const newOption = document.createElement("option");
        newOption.value = option.value;
        newOption.text = option.text;

        inputProbilisticMaterials.appendChild(newOption);
        fetchProbabilisticForms(option.value);
    });
}

function fetchProbabilisticForms(materialId) {
    console.log("Fetching forms for material ID:", materialId);

    for (let transportType in transportTypes) {
        const targetDiv = document.getElementById(transportTypes[transportType]);
        const uniqueFormId = `form_${transportType}_${materialId}`;

        if (document.getElementById(uniqueFormId)) continue;

        fetch(`/material/${materialId}/form/${transportType}`)
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    const wrapper = document.createElement("div");
                    wrapper.id = uniqueFormId;
                    wrapper.classList.add("material-form", "hidden");
                    wrapper.dataset.materialId = materialId;
                    // wrapper.innerHTML = `<h4>${materialId} ${transportType}</h4>${data.form}`;
                    targetDiv.appendChild(wrapper);
                }
            })
            .catch(error => {
                console.error("Erreur lors de la récupération du formulaire :", error);
            });
    }
}

function showForm(){
    const selectedId = inputProbilisticMaterials.value;

    document.querySelectorAll(".material-form").forEach(form => {
        form.classList.add("hidden");
    });

    document.querySelectorAll(`.material-form[data-material-id="${selectedId}"]`).forEach(form => {
        form.classList.remove("hidden");
    });
}

