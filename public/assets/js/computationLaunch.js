function launchComputation(inputID, nqual, materialNames, length) {
    let confirmation = confirm(translations.confirmLaunch);
    if (!confirmation) {
        return;
    }

    let epLayers = [0.0];
    let nlayers = 1;
    let jsonData = {};

    if (nqual > 1) {
        let useMultilayer = confirm(translations.confirmLayers);
        if (useMultilayer) {
            for(let i = 1;i<nqual;i++){
                let name = materialNames[i];
                let promptText = translations.promptLayerThickness
                    .replace('__NAME__', name)
                    .replace('__INDEX__', i)
                    .replace('__MAX__', length);

                let ep = prompt(promptText);
                if(ep ===null || isNaN(parseFloat(ep))){
                    alert(translations.invalidValue);
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