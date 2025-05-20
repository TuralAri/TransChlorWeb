//INIT OF ALL NEEDED FORM VALUES
let saturatedWaterInput;//text1 in vb code
let hydrationRateInput;//text44 in vb code
let airContentInput;//textbox2 in vb code
let freshConcreteDensityInput; //textbox1 in VB code
let cementContentInput;// text2 in vb code
let aggregateContentInput;//label 92 in vb code
let ECInput;// textbox 3 in vb
let cementDensityInput;//Textbox 26 in vb code
let aggregateDensityInput;// textbox25 in vb code
let dclToValueBasedOnEc; //checkbox 6 in vb code

//Here is what we'll call our main after defining all functions
document.addEventListener("DOMContentLoaded", (event) => {
    saturatedWaterInput = document.getElementById("material_form_saturatedWaterContent"); //text1 in vb code
    hydrationRateInput = document.getElementById("material_form_hydrationRate"); //text44 in vb code
    airContentInput = document.getElementById("material_form_airContent"); //textbox2 in vb code
    freshConcreteDensityInput = document.getElementById("material_form_freshConcreteDensity"); //textbox1 in VB code
    cementContentInput = document.getElementById("material_form_cementContent"); // text2 in vb code
    aggregateContentInput = document.getElementById("material_form_aggregateContent");//label 92 in vb code
    ECInput = document.getElementById("material_form_ec");// textbox 3 in vb
    cementDensityInput = document.getElementById("material_form_cementDensity"); //Textbox 26 in vb code
    aggregateDensityInput = document.getElementById("material_form_aggregateDensity"); // textbox25 in vb code
    dclToValueBasedOnEc = document.getElementById("material_form_dclToValueBasedOnEc") //Checkbox 6 in vb code

    //Loading listeners for inputs that need it

    //List of each inputs to execute functions when they are changed
    const inputsSaturatedWater = [cementContentInput, ECInput, hydrationRateInput, airContentInput].filter(Boolean);
    const inputsAggregateContent = [cementContentInput, cementDensityInput, ECInput, airContentInput, aggregateDensityInput].filter(Boolean);
    const inputsFreshConcrete = [cementContentInput, aggregateContentInput, ECInput].filter(Boolean);

    //If the same input is used in one of the condition it will execute in the order they were registered
    // (inputsSaturatedWater before inputsAggregateContent and finally inputsFreshConcrete)
    for (let input of inputsSaturatedWater) {
        input.addEventListener("change", calculateSaturatedWaterContent);
    }

    for (let input of inputsAggregateContent) {
        input.addEventListener("change", calculateAggregateContent);
    }

    for(let input of inputsFreshConcrete){
        input.addEventListener("change", calculateFreshConcreteContent)
    }
});


for(let i=0;i<=8;i++){
    addFormListener(i);
}

//END INIT OF ALL NEEDED FORM VALUES

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
    const cementContent = cementContentInput.value;
    const ec = ECInput.value;
    const hydrationRate = hydrationRateInput.value;
    const airContent = airContentInput.value;

    saturatedWaterInput.value =
        (parseFloat(cementContent) * parseFloat(ec))
        - (0.17 * parseFloat(hydrationRate) * parseFloat(cementContent))
        + (10 * parseFloat(airContent));
}

function calculateAggregateContent(){
    const cementContent = cementContentInput.value;
    const cementDensity = cementDensityInput.value;
    const ec = ECInput.value;
    const airContent = airContentInput.value;
    const aggregateDensity = aggregateDensityInput.value;

    aggregateContentInput.value = (1
        - parseFloat(cementContent) / parseFloat(cementDensity)
        - parseFloat(ec) * parseFloat(cementContent)/1000
        - parseFloat(airContent) / 100
    ) * parseFloat(aggregateDensity);
}

function calculateFreshConcreteContent(){
    const cementContent = cementContentInput.value;
    const aggregateContent = aggregateContentInput.value;
    const ec = ECInput.value;

    freshConcreteDensityInput.value = (parseFloat(cementContent) + parseFloat(aggregateContent) + parseFloat(ec) * cementContent);
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
            calculateFreshConcreteContent();
        })
        .catch(error => {
            console.error("Couldn't load permeability data:", error);
        });
}