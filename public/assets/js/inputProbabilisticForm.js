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

    // Avant envoi du formulaire global
    document.getElementById('input_form').addEventListener('submit', e => {
        gatherSubFormsData();
    });
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
                    wrapper.innerHTML = `<h4>${materialId} ${transportType}</h4>${data.form}`;

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

function gatherSubFormsData() {
    const data = {};

    document.querySelectorAll('.material-form').forEach(formDiv => {
        const materialId = formDiv.dataset.materialId;
        const formType = formDiv.id.split('_')[1];

        data[materialId] = data[materialId] || {};
        data[materialId][formType] = {};

        const inputs = formDiv.querySelectorAll('input, select, textarea');

        inputs.forEach(input => {
            let name = input.name;
            let value = input.value;

            const matches = name.match(/\[(.+)\]$/);
            if (matches && matches[1]) {
                const fieldName = matches[1];
                data[materialId][formType][fieldName] = value;
            } else {
                data[materialId][formType][name] = value;
            }
        });
    });

    const hiddenField = document.getElementById('input_form_probabilisticData');
    if (hiddenField) {
        hiddenField.value = JSON.stringify(data);
    } else {
        console.warn('Champ caché probabilisticData non trouvé');
    }
}



