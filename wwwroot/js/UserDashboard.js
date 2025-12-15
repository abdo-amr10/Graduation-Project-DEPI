document.querySelectorAll('.nav-item').forEach(item => {
    item.addEventListener('click', function () {
        document.querySelectorAll('.nav-item').forEach(i => i.classList.remove('active'));
        this.classList.add('active');
    });
});

document.querySelectorAll('.course-card, .opportunity-item').forEach(card => {
    card.addEventListener('click', function () {
        this.style.transform = 'scale(0.98)';
        setTimeout(() => {
            this.style.transform = '';
        }, 100);
    });
});
    </script >
    <script>
        const btn = document.getElementById('darkModeBtn');
    btn.addEventListener('click', () => {
            document.body.classList.toggle('dark-mode');
        btn.textContent = document.body.classList.contains('dark-mode') ? '☀️' : '🌙';
        // حفظ الحالة
        localStorage.setItem('darkMode', document.body.classList.contains('dark-mode'));
    });

    // تحميل الحالة عند الفتح
    window.addEventListener('load', () => {
        if (localStorage.getItem('darkMode') === 'true') {
            document.body.classList.add('dark-mode');
        btn.textContent = '☀️';
        }
    });