// wwwroot/js/Admin/indexcourse.js
(function () {
    "use strict";

    let courses = [];

    const $ = (sel) => document.querySelector(sel);
    const escapeHtml = (unsafe) => {
        if (unsafe === undefined || unsafe === null) return "";
        return String(unsafe)
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    };

    function getRowHtml(course) {
        const id = course.id ?? course.Id ?? "";
        const name = course.name ?? course.Name ?? "";
        const code = (course.code || course.Code || course.CourseCode || "") + "";
        const faculty = course.faculty || (course.Faculty && (course.Faculty.Name || course.Faculty)) || "";
        const department = course.department || (course.Department && (course.Department.Name || course.Department)) || "";
        const semester = course.semester || course.Semester || "";

        return `
        <tr data-id="${escapeHtml(id)}">
            <td>
                <div class="course-name">${escapeHtml(name)}</div>
                <div class="course-code">${escapeHtml(code)}</div>
            </td>
            <td>${escapeHtml(faculty)}</td>
            <td>${escapeHtml(department)}</td>
            <td>${escapeHtml(code)}</td>
            <td>${escapeHtml(semester)}</td>
            <td>
                <div class="action-buttons">
                    <button class="btn-icon btn-edit" title="Edit" data-id="${escapeHtml(id)}">✏️</button>
                    <button class="btn-icon btn-delete" title="Delete" data-id="${escapeHtml(id)}">🗑️</button>
                </div>
            </td>
        </tr>`;
    }

    function renderCourses(filtered = courses) {
        const tbody = $('#coursesTableBody');
        const empty = $('#emptyState');
        if (!tbody) return;

        if (!filtered || filtered.length === 0) {
            tbody.innerHTML = "";
            if (empty) empty.style.display = "block";
            return;
        }

        if (empty) empty.style.display = "none";
        tbody.innerHTML = filtered.map(getRowHtml).join('');
        attachRowButtons();
    }

    function attachRowButtons() {
        document.querySelectorAll('.btn-edit').forEach(b => {
            b.removeEventListener('click', onEditClick);
            b.addEventListener('click', onEditClick);
        });

        document.querySelectorAll('.btn-delete').forEach(b => {
            b.removeEventListener('click', onDeleteClick);
            b.addEventListener('click', onDeleteClick);
        });
    }

    function onEditClick(e) {
        const id = e.currentTarget.dataset.id;
        if (!id) return;
        window.location.href = `/admin/courses/edit/${encodeURIComponent(id)}`;
    }

    async function onDeleteClick(e) {
        const id = e.currentTarget.dataset.id;
        if (!id) return;
        const ok = confirm('Are you sure you want to delete this course?');
        if (!ok) return;

        // Attempt AJAX delete with antiforgery token
        const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
        const token = tokenInput ? tokenInput.value : null;
        const url = `/admin/courses/delete/${encodeURIComponent(id)}`;

        try {
            // Preferred: send as form-encoded body (ASP.NET expects token in form field)
            const formData = new URLSearchParams();
            if (token) formData.append('__RequestVerificationToken', token);

            const res = await fetch(url, {
                method: 'POST',
                headers: {
                    'Accept': 'text/html, application/json'
                },
                body: formData
            });

            if (res.ok) {
                // removed successfully — update UI
                courses = courses.filter(c => String(c.id ?? c.Id) !== String(id));
                filterCourses();
                return;
            } else {
                // non-ok response (e.g. 400/500) — fallback to standard form submit
                console.warn('Delete returned status', res.status);
                fallbackFormSubmit(id, tokenInput);
            }
        } catch (err) {
            console.error('Delete error', err);
            fallbackFormSubmit(id, tokenInput);
        }
    }

    function fallbackFormSubmit(id, tokenInput) {
        // build a form and submit it so server receives antiforgery token server-side
        const form = document.createElement('form');
        form.method = 'post';
        form.action = `/admin/courses/delete/${encodeURIComponent(id)}`;

        if (tokenInput) {
            const clone = tokenInput.cloneNode(true);
            form.appendChild(clone);
        } else {
            // no token on page — create empty hidden input (server will reject if antiforgery required)
            const inp = document.createElement('input');
            inp.type = 'hidden';
            inp.name = '__RequestVerificationToken';
            inp.value = '';
            form.appendChild(inp);
        }

        document.body.appendChild(form);
        form.submit();
    }

    async function fetchCoursesFromApi() {
        try {
            const res = await fetch('/admin/courses/list'); // optional endpoint if you implement
            if (!res.ok) throw new Error('Network response not ok');
            const data = await res.json();
            courses = Array.isArray(data) ? data : [];
            renderCourses();
        } catch (err) {
            console.warn('Could not fetch /admin/courses/list — falling back to server data if present.', err);
            renderCourses(courses);
        }
    }

    function filterCourses() {
        const qEl = $('#searchInput');
        const fEl = $('#facultyFilter');

        const q = qEl ? qEl.value.trim().toLowerCase() : "";
        const faculty = fEl ? fEl.value : "";

        const filtered = courses.filter(c => {
            const name = (c.name || c.Name || "").toString().toLowerCase();
            const code = ((c.code || c.Code || c.CourseCode) || "").toString().toLowerCase();
            const fac = (c.faculty || (c.Faculty && (c.Faculty.Name || c.Faculty)) || "").toString();

            const matchesQ = !q || name.includes(q) || code.includes(q) || (c.instructor || "").toLowerCase().includes(q);
            const matchesFaculty = !faculty || fac === faculty;
            return matchesQ && matchesFaculty;
        });

        renderCourses(filtered);
    }

    function debounce(fn, wait) {
        let t;
        return function (...args) {
            clearTimeout(t);
            t = setTimeout(() => fn.apply(this, args), wait);
        };
    }

    function init() {
        const search = $('#searchInput');
        const faculty = $('#facultyFilter');

        if (search) search.addEventListener('input', debounce(filterCourses, 180));
        if (faculty) faculty.addEventListener('change', filterCourses);

        if (window.__courses && Array.isArray(window.__courses)) {
            courses = window.__courses;
            renderCourses();
        } else {
            fetchCoursesFromApi();
        }
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();
