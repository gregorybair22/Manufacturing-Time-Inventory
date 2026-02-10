// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Move Bootstrap modals to body when opening so backdrop (dim mask) appears above page content
// and the modal stays on top and clickable. Restore modal to its original DOM position after
// close so the modal's position does not change visibly; restore is deferred so it happens
// after the close animation has finished and the user never sees the move.
document.addEventListener('DOMContentLoaded', function () {
  document.querySelectorAll('.modal').forEach(function (modal) {
    modal.addEventListener('show.bs.modal', function () {
      if (modal.parentNode !== document.body) {
        modal._modalRestore = { parent: modal.parentNode, next: modal.nextElementSibling };
        document.body.appendChild(modal);
      }
    });
    modal.addEventListener('hidden.bs.modal', function () {
      if (!modal._modalRestore || modal.parentNode !== document.body) return;
      var r = modal._modalRestore;
      function restore() {
        if (modal.parentNode === document.body && r.parent) {
          r.parent.insertBefore(modal, r.next);
          modal._modalRestore = null;
        }
      }
      requestAnimationFrame(function () { requestAnimationFrame(restore); });
    });
  });
});
