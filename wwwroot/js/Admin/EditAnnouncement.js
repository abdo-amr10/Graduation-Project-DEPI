document.addEventListener("DOMContentLoaded", function () {

    const form = document.getElementById("editAnnouncementForm");
    const updateBtn = document.getElementById("updateBtn");

    if (!form) return;

    form.addEventListener("submit", function () {
        if (updateBtn) {
            updateBtn.disabled = true;
            updateBtn.textContent = "Updating...";
        }
    });
});
