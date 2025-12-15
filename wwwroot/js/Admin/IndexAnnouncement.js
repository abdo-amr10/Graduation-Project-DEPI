// wwwroot/js/AnnouncementsIndex.js
(function () {
    let deleteAnnouncementId = null;

    function openDeleteModal(id) {
        deleteAnnouncementId = id;
        const modal = document.getElementById('deleteModal');
        if (!modal) return;
        modal.style.display = 'flex';
        modal.setAttribute('aria-hidden', 'false');
    }

    function closeDeleteModal() {
        deleteAnnouncementId = null;
        const modal = document.getElementById('deleteModal');
        if (!modal) return;
        modal.style.display = 'none';
        modal.setAttribute('aria-hidden', 'true');
    }

    async function confirmDelete() {
        if (!deleteAnnouncementId) return closeDeleteModal();

        // find the form that has data-announcement-id == deleteAnnouncementId
        const formSelector = 'form.delete-form[data-announcement-id="' + deleteAnnouncementId + '"]';
        const form = document.querySelector(formSelector);

        if (form) {
            // try to submit the actual form (this will include antiforgery token)
            try {
                const btn = form.querySelector('.btn-delete[data-id="' + deleteAnnouncementId + '"]');
                if (btn) {
                    btn.disabled = true;
                    btn.textContent = 'Deleting...';
                }
                form.submit(); // normal POST to controller
                return;
            } catch (err) {
                console.warn('Form submit failed, falling back to fetch', err);
            }
        }

        // fallback: send fetch POST to delete endpoint and include antiforgery token if found on page
        const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
        const token = tokenInput ? tokenInput.value : null;

        const headers = { 'Content-Type': 'application/x-www-form-urlencoded' };
        if (token) headers['RequestVerificationToken'] = token;

        try {
            await fetch('/admin/announcements/delete/' + deleteAnnouncementId, {
                method: 'POST',
                headers: headers,
                body: ''
            });
        } catch (e) {
            // ignore
        } finally {
            // reload to reflect change (or you can remove card from DOM instead)
            window.location.reload();
        }
    }

    // attach handlers to delete buttons
    function wireDeleteButtons() {
        const deleteButtons = document.querySelectorAll('.btn-delete[data-id]');
        deleteButtons.forEach(btn => {
            btn.removeEventListener('click', onDeleteBtnClick);
            btn.addEventListener('click', onDeleteBtnClick);
        });
    }

    function onDeleteBtnClick(e) {
        const id = this.getAttribute('data-id') || this.dataset.id;
        if (!id) return;
        openDeleteModal(id);
    }

    document.addEventListener('DOMContentLoaded', function () {
        wireDeleteButtons();

        const confirmBtn = document.getElementById('confirmDeleteBtn');
        const cancelBtn = document.getElementById('cancelDeleteBtn');
        if (confirmBtn) confirmBtn.addEventListener('click', confirmDelete);
        if (cancelBtn) cancelBtn.addEventListener('click', closeDeleteModal);

        // Accessibility: allow Esc to close modal
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') {
                const modal = document.getElementById('deleteModal');
                if (modal && modal.style.display === 'flex') closeDeleteModal();
            }
        });
    });

    // expose for inline usage (optional)
    window.openDeleteModal = openDeleteModal;
    window.closeDeleteModal = closeDeleteModal;
    window.confirmDelete = confirmDelete;
})();
