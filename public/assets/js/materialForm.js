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

//Here is what we'll call our main after defining all functions

//we have 8 formsPart so we're activating each one of them
for(let i=0;i<=8;i++){
    addFormListener(i);
}