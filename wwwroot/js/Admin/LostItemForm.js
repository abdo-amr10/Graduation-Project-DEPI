// wwwroot/js/LostItemForm.js
// small helper to preview image selected in create/edit forms

function clearPreview() {
    const preview = document.getElementById('imagePreview');
    if (!preview) return;
    const img = preview.querySelector('img');
    if (img) img.src = '';
    preview.style.display = 'none';
    // clear the file input
    const input = document.getElementById('imageInput');
    if (input) input.value = '';
}

document.addEventListener('DOMContentLoaded', function () {
    const input = document.getElementById('imageInput');
    const preview = document.getElementById('imagePreview');

    if (!input || !preview) return;

    input.addEventListener('change', function (e) {
        const file = input.files && input.files[0];
        if (!file) {
            preview.style.display = 'none';
            return;
        }

        if (!file.type.startsWith('image/')) {
            alert('Please select an image file.');
            input.value = '';
            return;
        }

        const reader = new FileReader();
        reader.onload = function (ev) {
            const img = preview.querySelector('img');
            img.src = ev.target.result;
            preview.style.display = 'flex';
        };
        reader.readAsDataURL(file);
    });
});
