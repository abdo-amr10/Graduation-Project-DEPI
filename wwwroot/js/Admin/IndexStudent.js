// StudentIndex.js
(function () {
    "use strict";

    // Attach delete handlers to all delete buttons
    function attachDeleteHandlers() {
        const deleteButtons = document.querySelectorAll('.btn-delete');
        deleteButtons.forEach(btn => {
            if (btn.dataset.bound) return;
            btn.dataset.bound = '1';
            btn.addEventListener('click', onDeleteClick);
        });
    }

    async function onDeleteClick(e) {
        const btn = e.currentTarget;
        const id = btn.dataset.id;
        if (!id) return;

        const ok = confirm('Are you sure you want to delete this student? This action cannot be undone.');
        if (!ok) return;

        // disable & indicate
        btn.disabled = true;
        const originalText = btn.textContent;
        btn.textContent = 'Deleting...';

        // try to find the closest form (we render a form per row with antiforgery token)
        const form = btn.closest('form');
        if (form) {
            try {
                form.submit(); // normal POST with antiforgery token
                // don't re-enable (page will redirect)
                return;
            } catch (err) {
                console.warn('Form submit failed, falling back to fetch', err);
            }
        }

        // fallback: POST via fetch including antiforgery token if present
        try {
            const tokenEl = document.querySelector('input[name="__RequestVerificationToken"]');
            const token = tokenEl ? tokenEl.value : null;

            const headers = { 'Content-Type': 'application/x-www-form-urlencoded' };
            if (token) headers['RequestVerificationToken'] = token;

            const res = await fetch('/admin/users/delete/' + encodeURIComponent(id), {
                method: 'POST',
                headers: headers,
                body: ''
            });

            if (res.ok) {
                // remove card from DOM
                const card = document.querySelector('.student-card[data-id="' + id + '"]');
                if (card) card.remove();
                // re-attach (optional)
                attachDeleteHandlers();
            } else {
                alert('Delete failed. Server returned ' + res.status);
                btn.disabled = false;
                btn.textContent = originalText;
            }
        } catch (err) {
            console.error(err);
            alert('Delete failed (network). See console.');
            btn.disabled = false;
            btn.textContent = originalText;
        }
    }

    // Observes DOM changes (useful if server side paging or partial updates)
    function watchDomForNewButtons() {
        attachDeleteHandlers();
        const grid = document.getElementById('studentsGrid');
        if (!grid) return;
        const obs = new MutationObserver(() => attachDeleteHandlers());
        obs.observe(grid, { childList: true, subtree: true });
    }

    // init
    document.addEventListener('DOMContentLoaded', function () {
        attachDeleteHandlers();
        watchDomForNewButtons();
    });
})();
