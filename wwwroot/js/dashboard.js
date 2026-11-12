(function () {
  'use strict';

  var sidebar = document.getElementById('sidebar');
  var overlay = document.getElementById('sidebarOverlay');
  var toggle = document.getElementById('sidebarToggle');
  function setSidebar(open) {
    if (!sidebar) return;
    sidebar.classList.toggle('-translate-x-full', !open);
    if (overlay) overlay.classList.toggle('hidden', !open);
    if (toggle) toggle.setAttribute('aria-expanded', String(open));
  }
  if (toggle) toggle.addEventListener('click', function () { setSidebar(sidebar.classList.contains('-translate-x-full')); });
  if (overlay) overlay.addEventListener('click', function () { setSidebar(false); });
  document.addEventListener('keydown', function (e) { if (e.key === 'Escape') setSidebar(false); });

  document.querySelectorAll('[data-focus]').forEach(function (b) {
    b.addEventListener('click', function () {
      var t = document.querySelector(b.getAttribute('data-focus'));
      if (t) { t.scrollIntoView({ behavior: 'smooth', block: 'center' }); t.focus(); }
    });
  });

  var menuBtn = document.getElementById('locationMenuBtn');
  var menu = document.getElementById('locationDropdown');
  var label = document.getElementById('currentLocationLabel');
  if (menuBtn && menu) {
    menuBtn.addEventListener('click', function (e) {
      e.stopPropagation();
      var hidden = menu.classList.toggle('hidden');
      menuBtn.setAttribute('aria-expanded', String(!hidden));
    });
    document.addEventListener('click', function (e) {
      if (!menu.contains(e.target) && !menuBtn.contains(e.target)) menu.classList.add('hidden');
    });
    menu.querySelectorAll('[data-location]').forEach(function (b) {
      b.addEventListener('click', function () {
        if (label) label.textContent = b.getAttribute('data-location');
        menu.classList.add('hidden');
      });
    });
  }

  var qrTarget = document.getElementById('qrTarget');
  var qrCaption = document.getElementById('qrCaption');
  function renderQr(value) {
    if (!qrTarget || !value || typeof qrcode !== 'function') return; 
    try {
      var qr = qrcode(0, 'M');
      qr.addData(value);
      qr.make();
      qrTarget.innerHTML = qr.createImgTag(6, 0, 'Entry QR code');
    } catch (err) { /* keep fallback text */ }
  }
  if (qrTarget) renderQr(qrTarget.getAttribute('data-qr'));

  document.querySelectorAll('[data-qr-select]').forEach(function (b) {
    b.addEventListener('click', function () {
      renderQr(b.getAttribute('data-qr-select'));
      if (qrCaption) qrCaption.textContent = b.getAttribute('data-qr-label') || '';
      document.querySelectorAll('[data-qr-select]').forEach(function (o) {
        o.setAttribute('aria-pressed', String(o === b));
      });
    });
  });

  var tokenInput = document.querySelector('#antiForgery input[name="__RequestVerificationToken"]');
  document.addEventListener('click', function (e) {
    var btn = e.target.closest('[data-save-toggle]');
    if (!btn) return;
    var icon = btn.querySelector('.material-symbols-outlined');
    var url = btn.getAttribute('data-save-url');
    var removeRow = btn.hasAttribute('data-remove-row');

    function apply() {
      if (removeRow) {
        var row = btn.closest('[data-saved-row]');
        if (row) row.remove();
        return;
      }
      var saved = icon && icon.textContent.trim() === 'bookmark';
      if (icon) {
        icon.textContent = saved ? 'bookmark_border' : 'bookmark';
        icon.classList.toggle('text-secondary', !saved);
      }
      btn.setAttribute('aria-label', saved ? 'Save event' : 'Remove from saved');
    }

    if (!url) { apply(); return; }
    btn.disabled = true;
    fetch(url, {
      method: 'POST',
      headers: tokenInput ? { 'RequestVerificationToken': tokenInput.value } : {},
      credentials: 'same-origin'
    }).then(function (r) { if (r.ok) apply(); })
      .catch(function () { /* leave UI unchanged on failure */ })
      .finally(function () { btn.disabled = false; });
  });

  document.querySelectorAll('select[data-autosubmit]').forEach(function (sel) {
    sel.addEventListener('change', function () { if (sel.form) sel.form.requestSubmit(); });
  });
})();
