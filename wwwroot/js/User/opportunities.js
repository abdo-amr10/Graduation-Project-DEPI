// simple client-side interactions: search & modal details
(function () {
    const qInput = document.getElementById('q') || document.querySelector('.search');
    const jobsList = document.getElementById('jobsList');
    const modal = document.getElementById('modal');
    const closeModal = document.getElementById('closeModal');
    const modalTitle = document.getElementById('modalTitle');
    const modalContent = document.getElementById('modalContent');

    // Clicking job title opens modal with description + apply link
    document.body.addEventListener('click', function (e) {
        const titleEl = e.target.closest('.title');
        if (titleEl) {
            const card = titleEl.closest('.job-card');
            if (!card) return;
            const id = card.getAttribute('data-id');
            const title = titleEl.textContent.trim();
            const excerpt = card.querySelector('.excerpt') ? card.querySelector('.excerpt').textContent : '';
            const apply = card.querySelector('.apply-link') ? card.querySelector('.apply-link').href : '#';

            modalTitle.textContent = title;
            modalContent.innerHTML = `<p>${escapeHtml(excerpt)}</p><p style="margin-top:12px"><a class="apply-link" href="${apply}" target="_blank">Apply Now</a></p>`;
            modal.style.display = 'flex';
        }
    });

    // close modal
    if (closeModal) closeModal.addEventListener('click', () => modal.style.display = 'none');
    window.addEventListener('click', (e) => { if (e.target === modal) modal.style.display = 'none'; });

    // quick client search (filters visible cards)
    if (qInput) {
        qInput.addEventListener('input', function (e) {
            const q = (e.target.value || '').trim().toLowerCase();
            filterByQuery(q);
        });
    }

    function filterByQuery(q) {
        const cards = Array.from(document.querySelectorAll('.job-card'));
        if (!q) { cards.forEach(c => c.style.display = ''); return; }
        cards.forEach(c => {
            const title = (c.querySelector('.title')?.textContent || '').toLowerCase();
            const org = (c.querySelector('.meta')?.textContent || '').toLowerCase();
            const excerpt = (c.querySelector('.excerpt')?.textContent || '').toLowerCase();
            const show = title.includes(q) || org.includes(q) || excerpt.includes(q);
            c.style.display = show ? '' : 'none';
        });
    }

    function escapeHtml(text) {
        return String(text)
            .replace(/&/g, '&amp;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;');
    }

    // initialize sidebar animation
    (function initSidebar() {
        const sidebar = document.getElementById('leftSidebar');
        if (!sidebar) return;
        sidebar.addEventListener('mouseenter', () => sidebar.classList.add('expanded'));
        sidebar.addEventListener('mouseleave', () => sidebar.classList.remove('expanded'));

        const items = Array.from(sidebar.querySelectorAll('.nav-item'));
        function baseName(h) { if (!h) return ''; return h.split('#')[0].split('/').pop().split('?')[0]; }
        const current = (location.pathname.split('/').pop() || '').toLowerCase();

        items.forEach(i => {
            const href = i.dataset.href;
            const labelEl = i.querySelector('.nav-label');
            if (labelEl) i.setAttribute('title', labelEl.textContent.trim());
            if (href) {
                i.setAttribute('role', 'link');
                i.addEventListener('click', () => { window.location.href = href; });
                const target = baseName(href).toLowerCase();
                if (target && target === current) { i.classList.add('active'); }
            }
        });
    })();

})();
