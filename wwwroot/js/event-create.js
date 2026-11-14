(function () {
  'use strict';

  var form = document.getElementById('create-event-form');
  if (!form) return;

  var $ = function (id) { return document.getElementById(id); };
  var MAX_BYTES = 4 * 1024 * 1024;                        
  var TYPES = ['image/jpeg', 'image/png', 'image/webp'];

  var DAYS = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
  var MONTHS = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
  function pad(n) { return (n < 10 ? '0' : '') + n; }
  function fmtDate(d) {
    return DAYS[d.getDay()] + ', ' + pad(d.getDate()) + ' ' + MONTHS[d.getMonth()] + ' ' + d.getFullYear() +
           ' · ' + pad(d.getHours()) + ':' + pad(d.getMinutes());
  }
  function fmtSize(bytes) {
    return bytes >= 1048576 ? (bytes / 1048576).toFixed(1) + ' MB' : Math.max(1, Math.round(bytes / 1024)) + ' KB';
  }
  function val(id) { var el = $(id); return el ? el.value.trim() : ''; }

  var title = $('Title'), desc = $('Description');

  function updateTitle() {
    $('title-count').textContent = title.value.length + ' / 120';
    $('pv-title').textContent = title.value.trim() || 'Untitled event';
  }
  function updateDesc() {
    var n = desc.value.trim().length;
    $('desc-count').textContent = n >= 50 ? n + ' characters' : n + ' / 50 minimum';
  }
  function updateCategory() {
    var sel = $('CategoryId'), badge = $('pv-category');
    var text = sel && sel.value ? sel.options[sel.selectedIndex].text : '';
    badge.textContent = text;
    badge.classList.toggle('hidden', !text);
    badge.classList.toggle('inline-flex', !!text);
  }
  function updateLocation() {
    var venue = val('VenueName'), city = val('City');
    $('pv-location').textContent = venue ? (city ? venue + ', ' + city : venue) : (city || 'Venue to be set');
  }

  function updateDates() {
    var s = $('StartDate').value, e = $('EndDate').value;
    var start = s ? new Date(s) : null, end = e ? new Date(e) : null;
    $('pv-date').textContent = start && !isNaN(start) ? fmtDate(start) + ' SAST' : 'Date to be set';

    var badge = $('duration-badge'), err = $('date-error');
    var ok = start && end && !isNaN(start) && !isNaN(end) && end > start;
    if (ok) {
      var hours = (end - start) / 36e5;
      badge.textContent = 'Duration: ' + (hours < 48 ? hours.toFixed(1) + ' h' : (hours / 24).toFixed(1) + ' days');
    }
    badge.classList.toggle('hidden', !ok);
    err.classList.toggle('hidden', !(start && end && !ok));
  }

  function setStep(name, done) {
    var li = document.querySelector('[data-step="' + name + '"]');
    if (!li) return 0;
    var icon = li.querySelector('.step-icon');
    icon.textContent = done ? 'check_circle' : 'radio_button_unchecked';
    icon.classList.toggle('text-secondary', done);
    icon.classList.toggle('text-on-surface-variant', !done);
    return done ? 1 : 0;
  }
  function updateProgress() {
    var s = $('StartDate').value, e = $('EndDate').value;
    var count = 0;
    count += setStep('info', val('Title') && val('CategoryId') && val('Description').length >= 50);
    count += setStep('venue', val('VenueName') && val('StreetAddress') && val('City') && val('Province'));
    count += setStep('time', s && e && new Date(e) > new Date(s));
    $('progress-count').textContent = count + ' of 3 ready';
  }

  function refresh() { updateTitle(); updateDesc(); updateCategory(); updateLocation(); updateDates(); updateProgress(); }
  form.addEventListener('input', refresh);
  form.addEventListener('change', refresh);

  var input = $('CoverImage'), zone = $('cover-dropzone');
  var preview = $('cover-preview'), previewImg = $('cover-preview-img'), meta = $('cover-meta'), errEl = $('cover-error');
  var pvImg = $('pv-img'), pvEmpty = $('pv-img-empty');
  var objectUrl = null;

  function showError(msg) {
    errEl.textContent = msg;
    errEl.classList.toggle('hidden', !msg);
    errEl.classList.toggle('flex', !!msg);
  }
  function releaseUrl() { if (objectUrl) { URL.revokeObjectURL(objectUrl); objectUrl = null; } }
  function resetPreview() {
    releaseUrl();
    previewImg.removeAttribute('src');
    preview.classList.add('hidden'); preview.classList.remove('flex');
    pvImg.removeAttribute('src'); pvImg.classList.add('hidden'); pvEmpty.classList.remove('hidden');
  }
  function validate(file) {
    if (TYPES.indexOf(file.type) === -1) return 'Choose a JPG, PNG or WebP image.';
    if (file.size > MAX_BYTES) return 'That image is ' + fmtSize(file.size) + '. The limit is 4 MB.';
    return '';
  }
  function handleFile(file) {
    showError('');
    if (!file) { resetPreview(); return; }

    var problem = validate(file);
    if (problem) { input.value = ''; resetPreview(); showError(problem); return; }

    releaseUrl();
    objectUrl = URL.createObjectURL(file);

    previewImg.onload = function () {
      meta.textContent = file.name + ' · ' + previewImg.naturalWidth + '×' + previewImg.naturalHeight + ' · ' + fmtSize(file.size);
    };
    previewImg.src = objectUrl;
    meta.textContent = file.name + ' · ' + fmtSize(file.size);
    preview.classList.remove('hidden'); preview.classList.add('flex');

    pvImg.src = objectUrl; pvImg.classList.remove('hidden'); pvEmpty.classList.add('hidden');
  }

  input.addEventListener('change', function () { handleFile(input.files && input.files[0]); });
  $('cover-remove').addEventListener('click', function () { input.value = ''; showError(''); resetPreview(); input.focus(); });

  ['dragenter', 'dragover'].forEach(function (t) {
    zone.addEventListener(t, function (e) { e.preventDefault(); zone.classList.add('border-secondary', 'bg-surface-container'); });
  });
  ['dragleave', 'drop'].forEach(function (t) {
    zone.addEventListener(t, function (e) { e.preventDefault(); zone.classList.remove('border-secondary', 'bg-surface-container'); });
  });
  zone.addEventListener('drop', function (e) {
    var files = e.dataTransfer && e.dataTransfer.files;
    if (!files || !files.length) return;
    input.files = files;                    
    handleFile(files[0]);
  });

  window.addEventListener('beforeunload', releaseUrl);

  form.addEventListener('submit', function (e) {
    setTimeout(function () {
      if (e.defaultPrevented) return;       
      var btn = $('submit-btn');
      btn.disabled = true;
      $('btn-text').textContent = input.files && input.files.length ? 'Uploading…' : 'Creating…';
      $('btn-icon').classList.add('hidden');
      $('btn-spinner').classList.remove('hidden');
    }, 0);
  });

  refresh();
})();