// lostfound.js — improved sidebar behavior + page interactions
(function () {
    // Toggle add form mode (lost / found)
    window.toggleForm = function (mode) {
        const f = document.getElementById('addForm');
        const statusInput = document.getElementById('status');
        const title = document.getElementById('formTitle');
        if (mode) {
            statusInput.value = mode === 'found' ? 'found' : 'lost';
            title.textContent = mode === 'found' ? 'Add Found Item' : 'Add Lost Item';
            const placeField = document.getElementById('placeField');
            const placeInput = document.getElementById('place');
            if (placeField) {
                if (mode === 'found') { placeField.style.display = ''; placeInput.setAttribute('required', ''); document.getElementById('placeLabel').textContent = 'Found Location'; }
                else { placeField.style.display = 'none'; placeInput.removeAttribute('required'); }
            }
            f.style.display = 'block';
            try { document.querySelector('.main')?.scrollIntoView({ behavior: 'smooth', block: 'start' }); } catch (e) { }
            document.getElementById('type')?.focus();
        } else {
            f.style.display = f.style.display === 'none' || f.style.display === '' ? 'block' : 'none';
            if (f.style.display === 'block') try { document.querySelector('.main')?.scrollIntoView({ behavior: 'smooth', block: 'start' }); } catch (e) { }
            if (f.style.display === 'none') { statusInput.value = 'lost'; title.textContent = 'Add Item'; }
        }
    };

    // client-side preview + submit behavior
    const form = document.getElementById('createForm');
    if (form) {
        form.addEventListener('submit', function (e) {
            // create preview but do not prevent submit for required validation
            const fileInput = document.getElementById('image');
            const file = fileInput?.files?.[0];
            const type = document.getElementById('type').value;
            const owner = document.getElementById('owner').value;
            const place = document.getElementById('place').value;
            const contact = document.getElementById('contact').value;
            const status = document.getElementById('status').value || 'lost';
            const description = document.getElementById('description')?.value || '';

            // create preview immediately (non-blocking)
            if (file) {
                const reader = new FileReader();
                reader.onload = function (ev) {
                    createClientItem(ev.target.result, type, owner, place, contact, status, description);
                    // let normal submit proceed
                };
                reader.readAsDataURL(file);
            } else {
                createClientItem('/images/placeholder-item.svg', type, owner, place, contact, status, description);
            }
            // allow real submit to continue — server handles save
        });
    }

    function createClientItem(dataUrl, type, owner, place, contact, status, description) {
        const list = document.getElementById('lostList');
        if (!list) return;
        const newItem = document.createElement('div');
        newItem.className = 'item-card';
        newItem.dataset.status = status;
        const statusLabel = status === 'found' ? 'Found' : 'Lost';
        newItem.innerHTML = `
            <img src="${escapeHtml(dataUrl)}" alt="${escapeHtml(type)}">
            <div class="item-info">
                <p><strong>${escapeHtml(type)}</strong> ${owner ? <span style="font-size:13px;color:#7f8c8d;margin-left:8px">— ${escapeHtml(owner)}</span> : ''}</p>
                <p class="muted">Owner/Finder: ${escapeHtml(owner)} • Location: ${escapeHtml(place)} • ${new Date().toISOString().split('T')[0]}</p>
                ${description ? <div class="excerpt" style="margin-top:6px">${escapeHtml(description)}</div> : ''}
                <p class="muted">Contact: ${escapeHtml(contact)}</p>
            </div>
        `;
        list.prepend(newItem);
        try { document.querySelector('form').reset(); } catch (e) { }
        toggleForm();
        updateCounts();
        pushActivity({ img: dataUrl, title: type + ' (' + statusLabel + ')', owner: owner });
    }

    // activity feed
    window.pushActivity = function (obj) {
        const root = document.getElementById('recentActivity');
        if (!root) return;
        const el = document.createElement('div');
        el.className = 'recent-entry';
        if (typeof obj === 'object') {
            const img = document.createElement('img'); img.src = obj.img || '/images/placeholder-item.svg'; img.alt = obj.title || 'item';
            const info = document.createElement('div'); info.className = 'r-info';
            const title = document.createElement('div'); title.className = 'r-title'; title.textContent = ${ obj.title } — ${ obj.owner || '' };
            const time = document.createElement('div'); time.className = 'r-time'; time.textContent = new Date().toLocaleString();
            info.appendChild(title); info.appendChild(time);
            el.appendChild(img); el.appendChild(info);
        } else {
            el.textContent = ${ obj } — ${ new Date().toLocaleString() };
        }
        root.prepend(el);
    };

    // counts + filters
    window.updateCounts = function () {
        const list = document.getElementById('lostList');
        if (!list) return;
        const items = Array.from(list.querySelectorAll('.item-card'));
        const total = items.length;
        const lost = items.filter(i => (i.dataset.status || '').toLowerCase() === 'lost').length;
        const found = items.filter(i => (i.dataset.status || '').toLowerCase() === 'found').length;
        document.getElementById('totalCount').textContent = total;
        document.getElementById('lostCount').textContent = lost;
        document.getElementById('foundCount').textContent = found;
    };

    window.filterList = function (mode) {
        const list = document.getElementById('lostList');
        if (!list) return;
        const items = Array.from(list.querySelectorAll('.item-card'));
        items.forEach(i => {
            const s = (i.dataset.status || '').toLowerCase();
            if (mode === 'all') i.style.display = '';
            else if (mode === 'lost') i.style.display = (s === 'lost') ? '' : 'none';
            else if (mode === 'found') i.style.display = (s === 'found') ? '' : 'none';
        });
    };

    // small safe escaping helper
    function escapeHtml(s) {
        if (!s) return '';
        return String(s).replace(/[&<>"'`=\/]/g, function (c) {
            return { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;', '/': '&#x2F;', '=': '&#x3D;', '`': '&#x60;' }[c];
        });
    }

    // Sidebar interactions: hover expand + keyboard toggle + click navigation
    document.addEventListener('DOMContentLoaded', function () {
        // ensure static items have default status
        document.querySelectorAll('#lostList .item-card').forEach(el => { if (!el.dataset.status) el.dataset.status = 'found'; });
        updateCounts();

        // seed recent activity client-side (optional)
        pushActivity({ img: '/images/photo-wallet.svg', title: 'Wallet', owner: 'Found by Sara' });
        pushActivity({ img: '/images/photo-keys.svg', title: 'Set of Keys', owner: 'Found by Laila' });

        // Sidebar behavior
        const sidebar = document.getElementById('leftSidebar');
        if (!sidebar) return;

        sidebar.addEventListener('mouseenter', () => sidebar.classList.add('expanded'));
        sidebar.addEventListener('mouseleave', () => sidebar.classList.remove('expanded'));

        document.addEventListener('keydown', function (e) {
            if ((e.key === 't' || e.key === 'T') && document.activeElement.tagName.toLowerCase() === 'body') {
                sidebar.classList.toggle('expanded');
            }
        });

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
            i.setAttribute('tabindex', '0');
            i.addEventListener('keydown', function (ev) {
                if (ev.key === 'Enter' || ev.key === ' ') { ev.preventDefault(); i.click(); }
            });
        });
    });
})();