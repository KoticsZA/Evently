(function () {
  var pwd = document.getElementById('Password');
  if (!pwd) return;

  var rules = {
    length: document.getElementById('ruleLength'),
    upper: document.getElementById('ruleUppercase'),
    symbol: document.getElementById('ruleNumberOrSymbol')
  };
  var badge = document.getElementById('criteriaMetBadge');

  function setRule(el, ok) {
    var dot = el.querySelector('.rule-dot');
    el.classList.toggle('text-primary', ok);
    el.classList.toggle('text-on-surface-variant', !ok);
    if (dot) { dot.classList.toggle('bg-primary', ok); dot.classList.toggle('bg-outline', !ok); }
  }

  function evaluate() {
    var v = pwd.value;
    var a = v.length >= 8, b = /[A-Z]/.test(v), c = /[\d\W_]/.test(v);
    setRule(rules.length, a); setRule(rules.upper, b); setRule(rules.symbol, c);
    var all = a && b && c;
    badge.classList.toggle('hidden', !all);
    badge.classList.toggle('flex', all);
  }

  pwd.addEventListener('input', evaluate);
  evaluate();
})();
