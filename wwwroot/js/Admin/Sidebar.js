(function () {
    const menuItems = document.querySelectorAll(".menu-item");

    // 1) امسك اسم الصفحة الحالية من الـ URL
    const currentPath = window.location.pathname.toLowerCase();

    // 2) Loop على كل item ونشوف مين المفروض يبقى Active
    menuItems.forEach(item => {
        const key = item.dataset.key;

        if (!key) return;

        // Mapping URLs to menu keys
        const map = {
            "announcements": "announcements",
            "dashboard": "dashboard",
            "users": "users",
            "courses": "courses",
            "materials": "materials",
            "qa": "qa",
            "opportunities": "opportunities",
            "lost-found": "lost-found",
            "settings": "settings"
        };

        // check if url contains key
        if (currentPath.includes(key)) {
            menuItems.forEach(i => i.classList.remove("active"));
            item.classList.add("active");

            // store in localStorage
            localStorage.setItem("activeNav", key);
        }
    });


    // 3) لو مجرد فتح الصفحة ومفيش URL مطابق → نستخدم المخزن
    const saved = localStorage.getItem("activeNav");
    if (saved) {
        menuItems.forEach(item => {
            item.classList.toggle("active", item.dataset.key === saved);
        });
    }

})();

(function () {
    const menuItems = Array.from(document.querySelectorAll(".menu-item"));

    // helper: normalizes path (lowercase, no trailing slash)
    function normalizePath(p) {
        if (!p) return "";
        try {
            p = decodeURIComponent(p);
        } catch (e) { /* ignore */ }
        p = p.toLowerCase();
        if (p.endsWith("/")) p = p.slice(0, -1);
        return p;
    }

    const currentPath = normalizePath(window.location.pathname);

    // try to find best match by:
    // 1) exact match with data-route (if provided)
    // 2) contains match with data-route
    // 3) match by href inside <a> (exact or contains)
    // 4) fallback to data-key contains
    function findActiveItem() {
        // 1 & 2: data-route preferred (explicit mapping)
        for (const item of menuItems) {
            const route = item.dataset.route ? normalizePath(item.dataset.route) : null;
            if (!route) continue;
            if (currentPath === route) return item;          // exact
        }
        for (const item of menuItems) {
            const route = item.dataset.route ? normalizePath(item.dataset.route) : null;
            if (!route) continue;
            if (currentPath.includes(route)) return item;    // contains
        }

        // 3: check anchor hrefs inside items
        for (const item of menuItems) {
            const a = item.querySelector("a[href]");
            if (!a) continue;
            const href = normalizePath((new URL(a.getAttribute("href"), window.location.origin)).pathname);
            if (currentPath === href) return item;
        }
        for (const item of menuItems) {
            const a = item.querySelector("a[href]");
            if (!a) continue;
            const href = normalizePath((new URL(a.getAttribute("href"), window.location.origin)).pathname);
            if (currentPath.includes(href) && href !== "") return item;
        }

        // 4: fallback to data-key contains
        for (const item of menuItems) {
            const key = item.dataset.key ? item.dataset.key.toLowerCase() : null;
            if (!key) continue;
            if (currentPath.includes(key)) return item;
        }

        return null;
    }

    function setActive(item) {
        menuItems.forEach(i => i.classList.remove("active"));
        if (!item) return;
        item.classList.add("active");
        const saveKey = item.dataset.route || item.dataset.key || item.querySelector("a")?.getAttribute("href") || "";
        try {
            localStorage.setItem("activeNav", saveKey);
        } catch (e) { /* ignore storage error */ }
    }

    // 1) try to find active by path
    const found = findActiveItem();
    if (found) {
        setActive(found);
    } else {
        // 2) if nothing matched, try localStorage fallback
        try {
            const saved = localStorage.getItem("activeNav");
            if (saved) {
                // try to match saved to an item (by route, key or href)
                const normalizedSaved = normalizePath(saved);
                const fallback = menuItems.find(item => {
                    const route = item.dataset.route ? normalizePath(item.dataset.route) : null;
                    const key = item.dataset.key ? item.dataset.key.toLowerCase() : null;
                    const a = item.querySelector("a[href]");
                    const href = a ? normalizePath((new URL(a.getAttribute("href"), window.location.origin)).pathname) : null;

                    return (route && route === normalizedSaved)
                        || (href && href === normalizedSaved)
                        || (key && normalizedSaved.includes(key))
                        || (route && normalizedSaved.includes(route));
                });
                if (fallback) setActive(fallback);
            }
        } catch (e) { /* ignore storage error */ }
    }

    // 3) attach click handlers so clicking a menu item sets it active immediately and stores it
    menuItems.forEach(item => {
        item.addEventListener("click", function (e) {
            // if click is on a link, allow navigation but still set active locally
            setActive(item);
            // don't prevent default — navigation should proceed
        });

        // optional: keyboard accessibility (Enter/Space)
        item.addEventListener("keydown", function (e) {
            if (e.key === "Enter" || e.key === " ") {
                e.preventDefault();
                item.click();
            }
        });
    });

    // 4) optional: respond to popstate (back/forward) to update active state
    window.addEventListener("popstate", function () {
        const f = findActiveItem();
        setActive(f);
    });

})();
