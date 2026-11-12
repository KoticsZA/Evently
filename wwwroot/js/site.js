// Generic password visibility toggle: <button data-toggle-password="InputId"><span class="material-symbols-outlined">visibility</span></button>
document.querySelectorAll('[data-toggle-password]').forEach(function (btn) {
  var input = document.getElementById(btn.dataset.togglePassword);
  var icon = btn.querySelector('.material-symbols-outlined');
  if (!input || !icon) return;
  btn.addEventListener('click', function () {
    var show = input.type === 'password';
    input.type = show ? 'text' : 'password';
    icon.textContent = show ? 'visibility_off' : 'visibility';
  });
});
