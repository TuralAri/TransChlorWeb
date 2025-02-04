document.getElementById('initButton').addEventListener('click', function() {
    fetch('/meteo-form/init', {
        headers: {
            'Content-Type': 'application/json'
        }
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
            alert('Erreur lors de l\'initialisation');
        });
});