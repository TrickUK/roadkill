$(function() {
    $.each($('video'), function() {
        videojs(this, { 'controls': true, 'autoplay': false, 'preload': 'auto' }, function () {});
    });
});