// OpportunityEdit.js
// simple client-side niceties: show success message on submit (progressive enhancement).
// You may instead let server redirect to index after POST; this script is optional.

(function () {
    document.addEventListener('DOMContentLoaded', function () {
        const form = document.getElementById('opportunityForm');
        const successEl = document.getElementById('successMessage');

        if (!form) return;

        form.addEventListener('submit', function (e) {
            // allow normal submit to run (server-side saves)
            // but show a quick client-side feedback by disabling submit and showing "saving..."
            const submitBtn = form.querySelector('button[type="submit"]');
            if (submitBtn) {
                submitBtn.disabled = true;
                submitBtn.innerText = 'Saving...';
            }
        });

        // If you want to show a client-only success after an AJAX response,
        // implement fetch call here and display successEl.style.display = 'block';
    });
})();
