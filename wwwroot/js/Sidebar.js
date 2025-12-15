(function () {
    const sidebar = document.getElementById('leftSidebar');
    sidebar.addEventListener('mouseenter', () => sidebar.classList.add('expanded'));
    sidebar.addEventListener('mouseleave', () => sidebar.classList.remove('expanded'));

    // For touch devices or small screens, allow toggle on click of logo
    const logo = sidebar.querySelector('.logo-section');
    logo && logo.addEventListener('click', (e) => {
        // if screen is small, toggle expansion
        if (window.innerWidth <= 900) {
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
    });
})();