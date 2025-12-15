// EditStudent.js
(function () {
    "use strict";

    const photoInput = document.getElementById('photoInput');
    const previewBox = document.getElementById('photoPreview');
    const previewImg = previewBox ? previewBox.querySelector('img') : null;
    const placeholder = previewBox ? previewBox.querySelector('.placeholder') : null;
    const form = document.getElementById('studentEditForm');
    const deleteBtn = document.getElementById('btnDelete');
    const deleteForm = document.getElementById('deleteForm');

    function showPreview(file) {
        if (!previewBox) return;
        const reader = new FileReader();
        reader.onload = function (e) {
            if (!previewImg) {
                const img = document.createElement('img');
                img.src = e.target.result;
                previewBox.innerHTML = '';
                previewBox.appendChild(img);
            } else {
                previewImg.src = e.target.result;
                previewImg.style.display = 'block';
            }
            if (placeholder) placeholder.style.display = 'none';
        };
        reader.readAsDataURL(file);
    }

    function clearPreviewToPlaceholder() {
        if (!previewBox) return;
        if (previewImg) {
            previewImg.src = '';
            previewImg.style.display = 'none';
        }
        if (placeholder) placeholder.style.display = 'block';
    }

    if (photoInput) {
        photoInput.addEventListener('change', function (e) {
            const f = e.target.files && e.target.files[0];
            if (!f) {
                clearPreviewToPlaceholder();
                return;
            }

            const maxBytes = 2 * 1024 * 1024; // 2MB
            if (f.size > maxBytes) {
                alert('Image is too large. Max 2MB.');
                photoInput.value = '';
                return;
            }

            if (!f.type.startsWith('image/')) {
                alert('Only image files are allowed.');
                photoInput.value = '';
                return;
            }

            showPreview(f);
        });
    }

    if (deleteBtn && deleteForm) {
        deleteBtn.addEventListener('click', function () {
            const ok = confirm('⚠️ Are you sure you want to delete this student? This cannot be undone.');
            if (!ok) return;

            const verify = prompt('Type DELETE to confirm removal of this student:');
            if (verify === 'DELETE') {
                deleteForm.submit();
            } else {
                alert('Deletion cancelled.');
            }
        });
    }

    if (form) {
        form.addEventListener('submit', function (e) {
            const fullName = form.querySelector('[name="FullName"]');
            const email = form.querySelector('[name="Email"]');
            if (!fullName || !fullName.value.trim()) {
                e.preventDefault();
                alert('Full name is required.');
                fullName.focus();
                return false;
            }
            if (!email || !email.value.trim()) {
                e.preventDefault();
                alert('Email is required.');
                email.focus();
                return false;
            }
            return true;
        });
    }

})();
