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
                    wrapper.dataset.transportType = transportType;
                    wrapper.dataset.material = JSON.stringify(data.materialData);
                    wrapper.innerHTML = `${data.form}`;

                    targetDiv.appendChild(wrapper);

                    //adding an event listener to the standard deviation input
                    const stdInput = wrapper.querySelector('[name$="[standardDeviation]"]');
                    if (stdInput) {
                        stdInput.addEventListener("input", () => {
                            handleStdDevChange(wrapper, stdInput);
                        });
                    }
                }
            })
            .catch(error => {
                console.error("Erreur lors de la récupération du formulaire :", error);
            });
    }
}

function handleStdDevChange(wrapper, stdInput) {
    const material = JSON.parse(wrapper.dataset.material || '{}');
    const type = wrapper.dataset.transportType;

    const std = parseFloat(stdInput.value);
    if (isNaN(std)) return;

    const computed = computeValues(material, type, std);
    const updateField = (name, value) => {
        const input = wrapper.querySelector(`[name$="[${name}]"]`);
        if (input && value !== null && !isNaN(value)) {
            input.value = value;
        }
    };

    updateField("meanValue", computed.meanValue);
    updateField("standardDeviation", std);
    updateField("lambda", computed.lambda);
    updateField("ksi", computed.ksi);
    updateField("pMinus", computed.pMinus);
    updateField("pPlus", computed.pPlus);
    updateField("x1", computed.x1);
    updateField("x2", computed.x2);
}


function computeValues(material, type, std) {
    let mean = null;

    switch (type) {
        case 'waterVaporTransport':
            mean = material.d100;
            break;
        case 'capillarityTransport':
            const ec = material.ec;
            const a = 0.0000625 * (ec * ec) - 0.000104 * ec + 0.00003;
            const b = -0.015547 * (ec * ec) + 0.021655 * ec - 0.005652;
            mean = a * 100 + b;
            break;
        case 'ionicTransport':
            mean = material.dcl;
            break;
        case 'carbonation':
            const ecC = material.ec;
            const cementDensity = material.cementDensity;
            const numerator = cementDensity * (ecC - 0.3) * (1 - 0.7);
            const denominator = 1000 * (1 + (cementDensity * ecC / 1000));
            const expression = numerator / denominator;
            mean = 2.8 * expression * expression;
            break;
    }

    if (!mean || !std || std <= 0) return {};

    const mean2 = mean * mean;
    const std2 = std * std;

    const lambda = Math.log(mean2 / Math.sqrt(mean2 + std2));
    const ksi = Math.sqrt(Math.log((std2 / mean2) + 1));
    console.log(lambda);
    console.log(ksi);

    const sm = Math.exp(lambda) * (1 - Math.exp(-ksi));
    const sp = Math.exp(lambda) * (Math.exp(ksi) - 1);

    const pMinus = sp / (sp + sm);
    const pPlus = sm / (sp + sm);

    const x1 = Math.exp(lambda - ksi);
    const x2 = Math.exp(lambda + ksi);

    return {
        meanValue: mean,
        standardDeviation: std,
        lambda,
        ksi,
        pMinus,
        pPlus,
        x1,
        x2
    };
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



