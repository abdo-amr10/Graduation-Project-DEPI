// JS for file preview + client-side validation for the material create/edit form

(function () {
    const fileInput = document.getElementById('fileInput');
    const filePreview = document.getElementById('filePreview');
    const fileError = document.getElementById('fileError');
    const submitBtn = document.getElementById('submitBtn');

    if (!fileInput) return;

    const MAX_BYTES = 10 * 1024 * 1024; // 10 MB
    const ALLOWED = [
        'application/pdf',
        'application/msword',
        'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
        'application/vnd.ms-powerpoint',
        'application/vnd.openxmlformats-officedocument.presentationml.presentation',
        'application/zip',
        'text/plain',
        'image/jpeg',
        'image/png',
        'image/gif',
        'image/webp'
    ];

    function resetPreview() {
        filePreview.innerHTML = '<span class="placeholder">No file selected</span>';
        fileError.style.display = 'none';
        submitBtn.disabled = false;
    }

    function setError(msg) {
        fileError.textContent = msg;
        fileError.style.display = 'block';
        submitBtn.disabled = true;
    }

    function renderPreview(file) {
        filePreview.innerHTML = '';

        const meta = document.createElement('div');
        meta.className = 'meta';

        const name = document.createElement('div');
        name.textContent = file.name;
        name.style.fontWeight = '600';

        const size = document.createElement('div');
        size.textContent = `${(file.size / 1024 / 1024).toFixed(2)} MB`;

        meta.appendChild(name);
        meta.appendChild(size);

        // If image, show thumbnail
        if (file.type && file.type.startsWith('image/')) {
            const img = document.createElement('img');
            const reader = new FileReader();
            reader.onload = function (e) {
                img.src = e.target.result;
                filePreview.appendChild(img);
                filePreview.appendChild(meta);
            };
            reader.readAsDataURL(file);
        } else {
            // icon + meta
            const icon = document.createElement('div');
            icon.innerHTML = '📄';
            icon.style.fontSize = '28px';
            filePreview.appendChild(icon);
            filePreview.appendChild(meta);
        }
    }

    fileInput.addEventListener('change', function (e) {
        fileError.style.display = 'none';
        const file = fileInput.files && fileInput.files[0];
        if (!file) {
            resetPreview();
            return;
        }

        // size check
        if (file.size > MAX_BYTES) {
            setError('File is too large. Max allowed size is 10 MB.');
            return;
        }

        // type check (be permissive if unknown)
        if (ALLOWED.indexOf(file.type) === -1 && file.type !== '') {
            // If browser didn't provide type, allow by extension check
            const ext = file.name.split('.').pop().toLowerCase();
            const allowedExt = ['pdf', 'doc', 'docx', 'ppt', 'pptx', 'zip', 'txt', 'jpg', 'jpeg', 'png', 'gif', 'webp'];
            if (!allowedExt.includes(ext)) {
                setError('File type not allowed. Allowed: pdf, docx, pptx, zip, txt, images.');
                return;
            }
        }

        // ok
        renderPreview(file);
    });

})();
