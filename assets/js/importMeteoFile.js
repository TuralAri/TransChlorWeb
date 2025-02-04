document.getElementById('fileInput').addEventListener('change', function(e) {
    const formData = new FormData();
    formData.append('file', this.files[0]);

    fetch('/upload-meteo', {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => console.log('Fichier uploadé:', data))
        .catch(error => console.error('Erreur:', error));
});