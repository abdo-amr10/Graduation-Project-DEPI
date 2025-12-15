// show uploaded filename and size
document.addEventListener('DOMContentLoaded', function () {
    var fileInput = document.getElementById('fileInput');
    if (!fileInput) return;
    fileInput.addEventListener('change', function () {
        if (fileInput.files && fileInput.files.length > 0) {
            var f = fileInput.files[0];
            var help = fileInput.nextElementSibling;
            if (help && help.classList.contains('help-text')) {
                help.textContent = `${f.name} (${Math.round(f.size / 1024)} KB) - Allowed: pdf/docx/pptx/png/jpg/zip (will replace old file)`;
            }
        }
    });
});
