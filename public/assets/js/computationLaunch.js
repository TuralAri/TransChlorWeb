function launchComputation(inputID, nqual, materialNames){
    let epLayers = [0.0];
    let nlayers = 1;
    let jsonData = {};

    if (nqual > 1) {
        let useMultilayer = confirm("Are there many layers? (Click 'OK' for yes, 'Cancel' for no)");
        if (useMultilayer) {
            for(let i = 1;i<nqual;i++){
                let name = materialNames[i] || `couche no ${i}`;
                let ep = prompt(`Épaisseur de ${name} ? (max ${length} mm)`);
                if(ep ===null || isNaN(parseFloat(ep))){
                    alert("Valeur invalide. Calcul annulé.");
                    return;
                }

                epLayers.push(parseFloat(ep) + epLayers[i - 1]);
            }
            nlayers = epLayers.length;
        }
    }

    jsonData = {
      nlayers: nlayers,
      epLayers: epLayers,
    };

    fetch(`/inputs/${inputID}/compute`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(jsonData)
    }).then(response => {
        if(response.redirected){
            window.location.href = response.url;
        }
        else {
            response.text().then(alert);
        }
    })
}