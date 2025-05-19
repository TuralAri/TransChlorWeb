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

//Function that permits fetching selected aggregate type's data and insert it into the form
document.addEventListener('DOMContentLoaded', () => {
    const aggregateSelect = document.querySelector('#material_form_aggregateType');
    const heatCapacityInput = document.querySelector('#material_form_heatCapacity');
    const aggregateDensityInput = document.querySelector('#material_form_aggregateDensity');

    if (aggregateSelect) {
        aggregateSelect.addEventListener('change', () => {
            const selectedId = aggregateSelect.value;
            if (!selectedId) return;

            fetch(`/aggregate-type/${selectedId}`)
                .then(response => response.json())
                .then(data => {
                    if(data.heatCapacity !== -1 && data.aggregateDensity !== -1){
                        heatCapacityInput.value = data.heatCapacity ?? '';
                        heatCapacityInput.readOnly = true; //disables user input (view only)
                        heatCapacityInput.classList.add('bg-gray-100'); //Adds gray effect to the readonly input
                        aggregateDensityInput.value = data.aggregateDensity ?? '';
                        aggregateDensityInput.readOnly = true;//disables user input (view only)
                        aggregateDensityInput.classList.add('bg-gray-100');//Adds gray effect to the readonly input
                    }else{
                        heatCapacityInput.readOnly = false;//enables user input
                        heatCapacityInput.classList.remove('bg-gray-100')//removes gray effect from input
                        aggregateDensityInput.readOnly = false;//enables user input
                        aggregateDensityInput.classList.remove('bg-gray-100')//removes gray effect from input
                    }
                })
                .catch(error => {
                    console.error('Error while loading aggregate\'s data :', error);
                });
        });
    }
});

//Permits managing Permeability option (changing the whole data of the form)
document.addEventListener('DOMContentLoaded', () => {
    const permeabilityRadios = document.querySelectorAll('input[name="material_form[permeability]"]');

    if (permeabilityRadios.length > 0) {
        permeabilityRadios.forEach(radio => {
            radio.addEventListener('change', handlePermeabilityChange);
        });
    }
});

const fieldMappings = {
    d100Percent: 'material_form_d100Percent',
    dclTo: 'material_form_dclTo',
    heatCapacity: 'material_form_heatCapacity',
    surfaceTransferCoefficient: 'material_form_surfaceTransferCoefficient',
    cementContent: 'material_form_cementContent',
    ec: 'material_form_ec',
    freshConcreteDensity: 'material_form_freshConcreteDensity',
    hydrationRate: 'material_form_hydrationRate',
    airContent: 'material_form_airContent',
    ed: 'material_form_ed',
    toDiffusion: 'material_form_toDiffusion',
    toChlorideDiffusion: 'material_form_toChlorideDiffusion'
};

function handlePermeabilityChange() {
    const selected = document.querySelector('input[name="material_form[permeability]"]:checked');
    const selectedId = selected ? selected.value : null;

    if (!selectedId) return;

    if (parseInt(selectedId) === 1) {
        unlockFields();
    } else {
        fetchPermeabilityData(selectedId);
    }
}

//Unlocks the selected inputs
function unlockFields() {
    for (const fieldId of Object.values(fieldMappings)) {
        const input = document.getElementById(fieldId);
        if (input) {
            input.readOnly = false;
            input.classList.remove('bg-gray-100');
        }
    }
}

function calculateSaturatedWaterContent(){
    const saturatedWaterInput = document.getElementById("material_form_saturatedWaterContent");
    const cementContentInput = document.getElementById("material_form_cementContent");
    const hydrationRateInput = document.getElementById("material_form_hydrationRate");
    const ECInput = document.getElementById("material_form_ec");
    const airContentInput = document.getElementById("material_form_airContent");

    const cementContent = cementContentInput.value;
    const ec = ECInput.value;
    const hydrationRate = hydrationRateInput.value;
    const airContent = airContentInput.value;

    saturatedWaterInput.value =
        (parseFloat(cementContent) * parseFloat(ec))
        - (0.17 * parseFloat(hydrationRate) * parseFloat(cementContent))
        + (10 * parseFloat(airContent));
}

function calculateAggregateContent() {
    const aggregateContentInput = document.getElementById("material_form_aggregateContent");
    const cementContentInput = document.getElementById("material_form_cementContent");
    const cementDensityInput = document.getElementById("material_form_cementDensity");
    const ECInput = document.getElementById("material_form_ec");
    const airContentInput = document.getElementById("material_form_airContent");
    const aggregateDensityInput = document.getElementById("material_form_aggregateDensity");

    const cementContent = cementContentInput.value;
    const cementDensity = cementDensityInput.value;
    const ec = ECInput.value;
    const airContent = airContentInput.value;
    const aggregateDensity = aggregateDensityInput.value;

    const cementRatio = parseFloat(cementContent) / parseFloat(cementDensity);
    console.log("Cement ratio:", cementRatio);

    const ecLoss = parseFloat(ec) * parseFloat(cementContent) / 1000;
    console.log("Entrained content loss (ecLoss):", ecLoss);

    const airLoss = parseFloat(airContent) / 100;
    console.log("Air content loss (airLoss):", airLoss);

    const remainingVolume = 1 - cementRatio - ecLoss - airLoss;
    console.log("Remaining volume after losses:", remainingVolume);

    const aggregateContent = remainingVolume * parseFloat(aggregateDensity);
    console.log("Final aggregate content:", aggregateContent);

    aggregateContentInput.value = aggregateContent;
}


function calculateFreshConcreteContent(){

}

function fetchPermeabilityData(id) {
    fetch(`/permeability/${id}`)
        .then(response => response.json())
        .then(data => {
            for (const [key, fieldId] of Object.entries(fieldMappings)) {
                const input = document.getElementById(fieldId);
                if (input && data[key] !== undefined) {
                    input.value = data[key];
                    input.readOnly = true;
                    input.classList.add('bg-gray-100');
                }
            }
            //launch some calculations here
            calculateSaturatedWaterContent();
            calculateAggregateContent();
        })
        .catch(error => {
            console.error("Couldn't load permeability data:", error);
        });
}



//Here is what we'll call our main after defining all functions

//we have 8 formsPart so we're activating each one of them
for(let i=0;i<=8;i++){
    addFormListener(i);
}