// MASTER-DETAIL: dinamičko dodavanje/uklanjanje stavki naloga
(function () {
    function preindeksirajStavke() {
        var redovi = document.querySelectorAll('#tabelaStavki tbody tr.stavka-red');
        redovi.forEach(function (red, indeks) {
            red.querySelectorAll('[name]').forEach(function (polje) {
                polje.name = polje.name.replace(/Stavke\[\d+\]/, 'Stavke[' + indeks + ']');
            });
            var brojacCelija = red.querySelector('.redni-broj');
            if (brojacCelija) brojacCelija.textContent = indeks + 1;
        });
        azurirajUkupnuPovrsinu();
    }

    function izracunajPovrsinuReda(red) {
        var sirina = parseFloat(red.querySelector('.polje-sirina')?.value) || 0;
        var visina = parseFloat(red.querySelector('.polje-visina')?.value) || 0;
        var kolicina = parseFloat(red.querySelector('.polje-kolicina')?.value) || 0;
        var povrsina = (sirina / 1000) * (visina / 1000) * kolicina;
        var celijaPovrsina = red.querySelector('.prikaz-povrsine');
        if (celijaPovrsina) celijaPovrsina.textContent = povrsina.toFixed(2) + ' m²';
        return povrsina;
    }

    function azurirajUkupnuPovrsinu() {
        var ukupno = 0;
        document.querySelectorAll('#tabelaStavki tbody tr.stavka-red').forEach(function (red) {
            ukupno += izracunajPovrsinuReda(red);
        });
        var prikaz = document.getElementById('ukupnaPovrsinaPrikaz');
        if (prikaz) prikaz.textContent = ukupno.toFixed(2) + ' m²';
    }

    window.dodajStavku = function () {
        var sablon = document.getElementById('sablonStavke');
        var telo = document.querySelector('#tabelaStavki tbody');
        if (!sablon || !telo) return;

        var noviRed = sablon.content.cloneNode(true);
        telo.appendChild(noviRed);
        preindeksirajStavke();
        vezizaDogadjajeReda(telo.querySelector('tr.stavka-red:last-child'));
    };

    window.ukloniStavku = function (dugme) {
        var telo = document.querySelector('#tabelaStavki tbody');
        var brojRedova = telo.querySelectorAll('tr.stavka-red').length;
        if (brojRedova <= 1) {
            alert('Nalog mora sadržati bar jednu stavku.');
            return;
        }
        dugme.closest('tr').remove();
        preindeksirajStavke();
    };

    function vezizaDogadjajeReda(red) {
        if (!red) return;
        ['.polje-sirina', '.polje-visina', '.polje-kolicina'].forEach(function (selektor) {
            var polje = red.querySelector(selektor);
            if (polje) polje.addEventListener('input', azurirajUkupnuPovrsinu);
        });
    }

    document.addEventListener('DOMContentLoaded', function () {
        document.querySelectorAll('#tabelaStavki tbody tr.stavka-red').forEach(vezizaDogadjajeReda);
        azurirajUkupnuPovrsinu();
    });
})();

// JS VALIDACIJE SA REGULARNIM IZRAZIMA (dopuna server-side validaciji)
(function () {
    var regexTelefon = /^[0-9\/\-\s+]{6,20}$/;
    var regexBrojIndeksa = /^\d{2,4}\/\d{4}$/; // primer: 123/2022 (nije obavezno, samo primer za proveru)

    function oznaciPolje(polje, ispravno, poruka) {
        polje.classList.toggle('is-invalid', !ispravno);
        polje.classList.toggle('is-valid', ispravno && polje.value.length > 0);

        var poljeGreske = document.getElementById(polje.id + '-js-greska');
        if (poljeGreske) {
            poljeGreske.textContent = ispravno ? '' : poruka;
        }
    }

    document.addEventListener('DOMContentLoaded', function () {
        var poljeTelefon = document.getElementById('Telefon');
        if (poljeTelefon) {
            poljeTelefon.addEventListener('input', function () {
                if (poljeTelefon.value.trim() === '') {
                    poljeTelefon.classList.remove('is-invalid', 'is-valid');
                    return;
                }
                var ispravno = regexTelefon.test(poljeTelefon.value.trim());
                oznaciPolje(poljeTelefon, ispravno, 'Telefon mora sadržati samo cifre, razmak, / - + (6 do 20 karaktera).');
            });
        }
    });
})();
