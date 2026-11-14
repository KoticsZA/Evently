(function () {
  'use strict';

  var dataEl = document.getElementById('cityData');
  var province = document.querySelector('[data-province-select]');
  var city = document.querySelector('[data-city-select]');
  if (!dataEl || !province || !city) return;

  var citiesByProvince = JSON.parse(dataEl.textContent);

  function fillCities(keepSelection) {
    var cities = citiesByProvince[province.value] || [];
    var current = keepSelection ? city.value : '';

    city.innerHTML = '';

    var placeholder = document.createElement('option');
    placeholder.value = '';
    placeholder.textContent = province.value ? 'Select a city' : 'Select a province first';
    city.appendChild(placeholder);

    cities.forEach(function (name) {
      var option = document.createElement('option');
      option.value = name;
      option.textContent = name;
      if (name === current) option.selected = true;
      city.appendChild(option);
    });

    city.disabled = cities.length === 0;
  }

  province.addEventListener('change', function () {
    fillCities(false);                                        // a new province always clears the old city
    city.dispatchEvent(new Event('change', { bubbles: true }));
  });

  fillCities(true);                                           
})();
