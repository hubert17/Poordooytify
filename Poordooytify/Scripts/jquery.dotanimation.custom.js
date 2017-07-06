// Disable button and start dot animation
var form = $('#form-submit');
form.submit(function () {
    if (form.valid()) {
        if (isMobile) {
            alert('Naga-depende ang kadugayon sa pag-upload sa imong internet connection. Taasi gamay imong pasensya. Salamat sa pag-share. :)');
        }
        var $el = $(":submit", this);
        $el.text(form.attr("name"));
        submitAnimator($el);
    }
});

$(document).on('click', '.submit', function () {
    submitAnimator(this);
});

var submitAnimator = function (e) {
    var $el = $(e);
    //$el.prop('disabled', true); // Disables visually + functionally
    $('button, a, :submit').prop('disabled', true); // disable all buttons and links

    $el.attr('style', 'text-align: left');
    var elW = $el.width();
    $el.animate({
        width: elW + 40
    }, 200);

    // Animation will start at once
    $el.dotAnimation({
        speed: 300,
        dotElement: '.',
        numDots: 3
    });
}
