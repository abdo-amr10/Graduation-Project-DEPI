// OpportunityIndex.js (delete + filtering) - updated delete logic
(function () {
    function qs(sel) { return document.querySelector(sel); }
    function qsa(sel) { return Array.from(document.querySelectorAll(sel)); }

    const searchInput = qs('#searchInput');
    const typeFilter = qs('#typeFilter');

    function parseDataAttr(value) {
        if (!value) return '';
        try { return JSON.parse(value); } catch { return value; }
    }

    function getRowData(tr) {
        return {
            id: tr.dataset.id,
            title: parseDataAttr(tr.dataset.title || ''),
            type: tr.dataset.type || '',
            faculty: parseDataAttr(tr.dataset.faculty || ''),
            organization: parseDataAttr(tr.dataset.organization || '')
        };
    }

    function renderFilter() {
        const q = (searchInput && searchInput.value || '').trim().toLowerCase();
        const type = typeFilter ? typeFilter.value : '';

        const rows = qsa('#opportunitiesTable tbody tr');
        let shown = 0;
        rows.forEach(r => {
            if (!r.dataset.id) return;
            const d = getRowData(r);
            const matchesQ = q === '' || (d.title && d.title.toLowerCase().includes(q)) || (d.organization && d.organization.toLowerCase().includes(q));
            const matchesType = type === '' || d.type === type;
            if (matchesQ && matchesType) {
                r.style.display = '';
                shown++;
            } else {
                r.style.display = 'none';
            }
        });

        const emptyEl = document.querySelector('.empty-state');
        if (emptyEl) emptyEl.style.display = shown === 0 ? '' : 'none';
    }

    // Modal helpers
    window.openDeleteModal = function (id, titleJson) {
        const modal = qs('#deleteModal');
        const msg = qs('#deleteMessage');
        const idInput = qs('#deleteId');
        if (!modal || !msg || !idInput) return;
        let title = titleJson;
        try { title = (typeof titleJson === 'string') ? JSON.parse(titleJson) : titleJson; } catch { /* keep raw */ }
        msg.textContent = `Are you sure you want to delete "${title}"?`;
        idInput.value = id;
        modal.style.display = 'flex';
        modal.setAttribute('aria-hidden', 'false');
    };

    window.closeDeleteModal = function () {
        const modal = qs('#deleteModal');
        if (!modal) return;
        modal.style.display = 'none';
        modal.setAttribute('aria-hidden', 'true');
    };

    // Confirm delete -> AJAX POST (reads form.action & antiforgery token)
    window.confirmDelete = async function () {
        try {
            const id = qs('#deleteId').value;
            if (!id) return alert('Missing id');

            const deleteForm = qs('#deleteForm');
            if (!deleteForm) return alert('Delete form not found');

            // Get anti-forgery token input value
            const tokenInput = deleteForm.querySelector('input[name="__RequestVerificationToken"]');
            const token = tokenInput ? tokenInput.value : null;

            // Use the form's action (so view controls route)
            const url = deleteForm.action || '/admin/opportunities/delete';

            const formData = new FormData();
            formData.append('id', id);
            if (token) formData.append('__RequestVerificationToken', token);

            const res = await fetch(url, {
                method: 'POST',
                body: formData,
                credentials: 'same-origin',
                headers: token ? { 'RequestVerificationToken': token } : {}
            });

            if (!res.ok) {
                // Try to show useful debug info
                const txt = await res.text().catch(() => res.statusText);
                console.error('Delete failed:', res.status, txt);
                alert('Delete failed: ' + (txt || res.statusText));
                return;
            }

            // success -> remove row from DOM and re-run filter
            const row = document.querySelector(`tr[data-id="${id}"]`);
            if (row) {
                row.classList.add('deleting');
                setTimeout(() => {
                    row.remove();
                    renderFilter();
                }, 200);
            }

            closeDeleteModal();
        } catch (err) {
            console.error(err);
            alert('An error occurred while deleting.');
        }
    };

    // events
    if (searchInput) searchInput.addEventListener('input', renderFilter);
    if (typeFilter) typeFilter.addEventListener('change', renderFilter);

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') closeDeleteModal();
    });
})();
