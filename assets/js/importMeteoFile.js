document.getElementById('fileInput').addEventListener('change', function(e) {
    const formData = new FormData();
    formData.append('file', this.files[0]);
    console.log('file', this.files[0]);
    fetch('/upload-meteo', {
        method: 'POST',
        body: formData
    })
        .then(response => {

            console.log(response);
            if (!response.ok) {
                throw new Error('Erreur lors de l\'upload du fichier');
            }
            return response.json();
        })
        .then(() => {
            return fetch('/meteo-form/init', {
                headers: {
                    'Content-Type': 'application/json'
                }
            });
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('Erreur lors de la récupération des données');
            }
            return response.json();
        })
        .then(data => {
            if (data.error) {
                alert(data.error);
                return;
            }

            Object.keys(data).forEach(key => {
                let input = document.querySelector(`[name="meteo[${key}]"]`);
                if (input) {
                    input.value = data[key];
                }
            });
        })
        .catch(error => {
            console.error('Erreur:', error);
            alert('Une erreur est survenue');
        });
});