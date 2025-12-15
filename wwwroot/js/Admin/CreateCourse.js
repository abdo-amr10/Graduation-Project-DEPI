// ~/js/Admin/CreateCourse.js
// Sidebar + form small improvements
(function () {
    const sidebar = document.getElementById('sidebar');
    const menuItems = document.querySelectorAll('.menu-item');

    // set active from localStorage initially
    const activeKey = localStorage.getItem('activeNav');
    if (activeKey) {
        menuItems.forEach(i => i.classList.toggle('active', i.dataset.key === activeKey));
    }

    // Also mark active based on current URL (higher priority)
    const path = window.location.pathname.toLowerCase();
    if (path) {
        menuItems.forEach(item => {
            const key = (item.dataset.key || '').toLowerCase();
            if (!key) return;
            if (path.includes(key)) {
                menuItems.forEach(i => i.classList.remove('active'));
                item.classList.add('active');
                localStorage.setItem('activeNav', key);
            }
        });
    }

    menuItems.forEach(item => {
        // if the li contains an <a>, listen to its click so navigation still works
        const anchor = item.querySelector('a');
        const setActive = () => {
            if (item.dataset.key) {
                localStorage.setItem('activeNav', item.dataset.key);
            }
            menuItems.forEach(i => i.classList.remove('active'));
            item.classList.add('active');
        };

        if (anchor) {
            anchor.addEventListener('click', setActive);
        } else {
            item.addEventListener('click', setActive);
        }

        item.tabIndex = 0;
        item.addEventListener('keypress', e => {
            if (e.key === 'Enter') {
                if (anchor) anchor.click();
                else item.click();
            }
        });
    });

    if (sidebar) {
        sidebar.addEventListener('mouseenter', () => sidebar.classList.add('expanded'));
        sidebar.addEventListener('mouseleave', () => sidebar.classList.remove('expanded'));

        sidebar.addEventListener('click', (e) => {
            if (window.innerWidth <= 800) {
                sidebar.classList.toggle('expanded');
            }
        });
    }

    document.addEventListener('click', function (event) {
        if (window.innerWidth <= 1024 && sidebar) {
            if (!sidebar.contains(event.target) && sidebar.classList.contains('expanded')) {
                sidebar.classList.remove('expanded');
            }
        }
    });

    document.addEventListener('keydown', function (event) {
        if (event.key === 'Escape' && sidebar) {
            if (sidebar.classList.contains('expanded')) {
                sidebar.classList.remove('expanded');
            }
        }
    });
})();

// Form handling — **لا تمنع الإرسال**، فقط تهيئة خفيفة UX
(function () {
    const form = document.getElementById('addCourseForm');
    if (!form) return;

    // تأكد الزر نوعه submit في ال cshtml: <button type="submit">Create Course</button>
    form.addEventListener('submit', function (e) {
        // لا تستخدم e.preventDefault() هنا إذا تريد الإرسال للسيرفر
        // يمكنك هنا إضافة تحقق بسيط قبل الإرسال (مثال):
        // if (!document.getElementById('courseName').value.trim()) {
        //     e.preventDefault();
        //     alert('Enter course name');
        // }
    });
})();
