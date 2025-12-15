// Sidebar functionality
(function () {
    const sidebar = document.getElementById('sidebar');
    const menuItems = document.querySelectorAll('.menu-item');

    const activeKey = localStorage.getItem('activeNav');
    if (activeKey) {
        menuItems.forEach(i => i.classList.toggle('active', i.dataset.key === activeKey));
    }

    menuItems.forEach(item => {
        item.addEventListener('click', function () {
            menuItems.forEach(i => i.classList.remove('active'));
            this.classList.add('active');

            if (this.dataset.key) {
                localStorage.setItem('activeNav', this.dataset.key);
            }

            if (sidebar.classList.contains('expanded')) {
                sidebar.classList.remove('expanded');
            }
        });

        item.tabIndex = 0;
        item.addEventListener('keypress', e => {
            if (e.key === 'Enter') item.click();
        });
    });

    if (sidebar) {
        sidebar.addEventListener('mouseenter', () => sidebar.classList.add('expanded'));
        sidebar.addEventListener('mouseleave', () => sidebar.classList.remove('expanded'));
    }

    document.addEventListener('keydown', function (event) {
        if (event.key === 'Escape') {
            if (sidebar.classList.contains('expanded')) {
                sidebar.classList.remove('expanded');
            }
        }
    });
})();

// 🔥 REAL FORM SUBMISSION — allow MVC POST to work
document.getElementById('addAnnouncementForm')
    .addEventListener('submit', function () {
        // ❌ لا تعمل preventDefault هنا
        // ❌ لا تعمل أي redirect
        // ❌ لا تعمل أي alert
        // المتصفح سيقوم بالإرسال تلقائياً إلى الـ controller
    });
