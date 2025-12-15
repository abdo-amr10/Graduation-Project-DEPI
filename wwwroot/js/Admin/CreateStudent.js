// CreateStudent.js
(function () {
    "use strict";

    const photoInput = document.getElementById('photoInput');
    const preview = document.getElementById('photoPreview');
    const previewImg = preview ? preview.querySelector('img') : null;
    const placeholder = preview ? preview.querySelector('.placeholder') : null;
    const form = document.getElementById('studentForm');

    function showPreview(file) {
        if (!previewImg) return;
        const reader = new FileReader();
        reader.onload = function (e) {
            previewImg.src = e.target.result;
            previewImg.style.display = 'block';
            if (placeholder) placeholder.style.display = 'none';
        }
        reader.readAsDataURL(file);
    }

    function clearPreview() {
        if (!previewImg) return;
        previewImg.src = '';
        previewImg.style.display = 'none';
        if (placeholder) placeholder.style.display = 'block';
    }

    if (photoInput) {
        photoInput.addEventListener('change', function (e) {
            const f = e.target.files && e.target.files[0];
            if (!f) {
                clearPreview();
                return;
            }
            // basic file size/type checks
            const maxBytes = 2 * 1024 * 1024; // 2MB
            const allowed = ['image/jpeg', 'image/png', 'image/webp'];
            if (f.size > maxBytes) {
                alert('Image is too large. Try a smaller file (max 2MB).');
                photoInput.value = '';
                clearPreview();
                return;
            }
            if (!allowed.includes(f.type)) {
                // still allow other image types but warn
                if (!f.type.startsWith('image/')) {
                    alert('Only image files are allowed.');
                    photoInput.value = '';
                    clearPreview();
                    return;
                }
            }
            showPreview(f);
        });
    }

    // optional: client-side final validation before submit (lightweight)
    if (form) {
        form.addEventListener('submit', function (e) {
            // simple required checks (server still authoritative)
            const fullName = form.querySelector('[name="FullName"]');
            const email = form.querySelector('[name="Email"]');
            if (!fullName || !fullName.value.trim()) {
                e.preventDefault();
                alert('Full name is required.');
                fullName.focus();
                return;
            }
            if (!email || !email.value.trim()) {
                e.preventDefault();
                alert('Email is required.');
                email.focus();
                return;
            }
            // allow submit to server
        });
    }

})();

    (function () {
        function toggleField(buttonId, fieldId) {
            var btn = document.getElementById(buttonId);
            var fld = document.getElementById(fieldId);
            if (!btn || !fld) return;
            btn.addEventListener('click', function () {
                if (fld.type === 'password') {
                    fld.type = 'text';
                    btn.textContent = '🙈';
                } else {
                    fld.type = 'password';
                    btn.textContent = '👁️';
                }
            });
        }

        // attach to both fields
        toggleField('togglePwd', 'pwd');
    toggleField('toggleConfirmPwd', 'confirmPwd');
    })();
