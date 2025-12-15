// OpportunityCreate.js
// small client-side helpers (default posted date and show success message after client-side form submit if desired)

document.addEventListener('DOMContentLoaded', function () {
    // try common id variations for the PostedAt input
    var posted = document.getElementById('postedAt') || document.getElementById('PostedAt') || document.querySelector("input[type='date'][name$='PostedAt']");

    if (posted && !posted.value) {
        try {
            var today = new Date().toISOString().split('T')[0];
            posted.value = today;
            console.debug("OpportunityCreate: set default posted date to", today);
        } catch (e) {
            console.warn("OpportunityCreate: failed to set default date", e);
        }
    }

    // show success message if querystring ?saved=true (optional)
    var params = new URLSearchParams(window.location.search);
    if (params.get('saved') === 'true') {
        var msg = document.getElementById('successMessage');
        if (msg) {
            msg.style.display = 'block';
            setTimeout(function () { msg.style.display = 'none'; }, 3500);
        }
    }

    // Basic diagnostic: log submit attempts, but do NOT prevent default.
    var form = document.getElementById('opportunityForm');
    if (form) {
        form.addEventListener('submit', function (ev) {
            console.debug("OpportunityCreate: form submit event fired. form action:", form.action, "method:", form.method);
            // Do not call ev.preventDefault() here — we want the normal post to occur.
            // If you need to do AJAX in future, you can preventDefault and handle fetch/XHR here.
        }, { capture: true });
    } else {
        console.warn("OpportunityCreate: form with id 'opportunityForm' not found.");
    }
});
