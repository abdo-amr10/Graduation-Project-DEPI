// MaterialsIndex.js
(() => {
    // Grab anti-forgery token value from the hidden form
    function getRequestVerificationToken() {
        const form = document.getElementById('antiforgeryForm');
        if (!form) return null;
        const input = form.querySelector('input[name="__RequestVerificationToken"]');
        return input ? input.value : null;
    }

    // Delete handler: sends POST to /admin/materials/delete/{id}
    async function deleteMaterial(id, row) {
        const token = getRequestVerificationToken();
        if (!token) {
            alert('Unable to get anti-forgery token. Try reloading the page.');
            return;
        }

        if (!confirm('Are you sure you want to delete this material? This action cannot be undone.')) return;

        try {
            const res = await fetch(`/admin/materials/delete/${id}`, {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': token,
                    'X-Requested-With': 'XMLHttpRequest'
                },
                credentials: 'same-origin'
            });

            if (!res.ok) {
                const txt = await res.text();
                console.error('Delete failed', res.status, txt);
                alert('Delete failed. See console for details.');
                return;
            }

            // remove the row with a quick fade/slide
            row.style.transition = 'opacity 240ms, transform 240ms';
            row.style.opacity = '0';
            row.style.transform = 'translateX(-12px)';
            setTimeout(() => row.remove(), 300);

        } catch (err) {
            console.error(err);
            alert('Network error while deleting. Check console.');
        }
    }

    // wire up delete buttons
    function wireDeleteButtons() {
        const table = document.getElementById('materialsTableBody');
        if (!table) return;
        table.addEventListener('click', function (e) {
            const target = e.target.closest('.action-delete');
            if (!target) return;
            const id = target.dataset.id || target.getAttribute('data-id');
            // Some buttons might be anchors inside, try find parent tr
            let row = target.closest('tr');
            if (!id) {
                // fallback: data-id on tr
                id = row ? row.getAttribute('data-id') : null;
            }
            if (!id || !row) return;
            deleteMaterial(id, row);
        });
    }

    // Simple search/filter on client side for better UX
    function wireSearchAndFilter() {
        const search = document.getElementById('searchInput');
        const filter = document.getElementById('courseFilter');
        const tableBody = document.getElementById('materialsTableBody');

        if (!tableBody) return;

        function filterRows() {
            const term = search ? search.value.trim().toLowerCase() : '';
            const course = filter ? filter.value.trim().toLowerCase() : '';
            const rows = Array.from(tableBody.querySelectorAll('tr')).filter(r => !r.classList.contains('empty-row'));

            let anyVisible = false;
            rows.forEach(r => {
                const title = (r.querySelector('.material-title') ? r.querySelector('.material-title').textContent : '').toLowerCase();
                const courseCol = (r.children[1] ? r.children[1].textContent : '').toLowerCase();
                const matchesTerm = !term || title.includes(term) || courseCol.includes(term);
                const matchesCourse = !course || courseCol.includes(course);
                const visible = matchesTerm && matchesCourse;
                r.style.display = visible ? '' : 'none';
                if (visible) anyVisible = true;
            });

            // show empty row when nothing visible
            const empty = tableBody.querySelector('.empty-row');
            if (!anyVisible) {
                if (!empty) {
                    const tr = document.createElement('tr');
                    tr.className = 'empty-row';
                    tr.innerHTML = '<td colspan="5">No materials match your search.</td>';
                    tableBody.appendChild(tr);
                }
            } else if (empty) {
                empty.remove();
            }
        }

        if (search) search.addEventListener('input', filterRows);
        if (filter) filter.addEventListener('change', filterRows);
    }

    // initialize
    document.addEventListener('DOMContentLoaded', function () {
        wireDeleteButtons();
        wireSearchAndFilter();
    });
})();
