// wwwroot/js/shared/header-controls.js
(function () {
    const sidebar = document.getElementById('appSidebar');
    const toggleBtn = document.getElementById('toggleSidebarBtn');
    const mainWrap = document.getElementById('mainWrap');

    if (!sidebar) return;

    // helper to set expanded state
    function setExpanded(expanded) {
        if (expanded) {
            sidebar.style.width = getComputedStyle(document.documentElement).getPropertyValue('--sidebar-width-expanded') || '230px';
            sidebar.classList.add('expanded');
            if (mainWrap) mainWrap.classList.add('sidebar-pushed');
            try { localStorage.setItem('sidebarExpanded', 'true'); } catch (e) { }
        } else {
            sidebar.style.width = getComputedStyle(document.documentElement).getPropertyValue('--sidebar-width-collapsed') || '70px';
            sidebar.classList.remove('expanded');
            if (mainWrap) mainWrap.classList.remove('sidebar-pushed');
            try { localStorage.setItem('sidebarExpanded', 'false'); } catch (e) { }
        }
    }

    // restore state
    try {
        const stored = localStorage.getItem('sidebarExpanded');
        if (stored === 'true') setExpanded(true);
        else setExpanded(false);
    } catch (e) { setExpanded(false); }

    // mouse enter/leave quick expand
    let timer = null;
    sidebar.addEventListener('mouseenter', () => {
        clearTimeout(timer);
        setExpanded(true);
    });
    sidebar.addEventListener('mouseleave', () => {
        timer = setTimeout(() => setExpanded(false), 220);
    });

    // toggle button (header)
    if (toggleBtn) {
        toggleBtn.addEventListener('click', (e) => {
            const expanded = sidebar.classList.contains('expanded');
            setExpanded(!expanded);
        });
    }

    // nav active storage and click handling
    const navItems = Array.from(document.querySelectorAll('#appSidebar .nav-item'));
    navItems.forEach(it => {
        it.addEventListener('click', function (ev) {
            try { localStorage.setItem('appSidebarActive', it.dataset.key || ''); } catch (e) { }
            // if anchor has href, let navigation proceed; otherwise highlight
            navItems.forEach(x => x.classList.remove('active'));
            it.classList.add('active');
        });
    });

    // restore active from server ViewData active class or localStorage
    (function restoreActive() {
        // server side may already have set 'active' class in the partial; check first
        const serverOne = document.querySelector('#appSidebar .nav-item.active');
        if (serverOne) return;
        try {
            const key = localStorage.getItem('appSidebarActive');
            if (key) {
                const el = document.querySelector(`#appSidebar .nav-item[data-key="${key}"]`);
                if (el) {
                    navItems.forEach(x => x.classList.remove('active'));
                    el.classList.add('active');
                }
            }
        } catch (e) { }
    })();

    // accessibility: close on Escape
    document.addEventListener('keydown', (ev) => {
        if (ev.key === 'Escape') setExpanded(false);
    });

})();
