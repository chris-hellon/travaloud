var videoPlaying = false;
$("document").ready(function() {
    $('.videoContainer').on('click', function () {
        if (!videoPlaying)
        {
            $('video').get(0).play();
            $('.videoContainer i').removeClass('fa-circle-play').addClass('fa-circle-pause');
            $('.videoContainer .mask').fadeOut();
            videoPlaying = true;
        }
        else {
            $('video').get(0).pause();
            $('.videoContainer .mask').fadeIn();
            $('.videoContainer i').removeClass('fa-circle-pause').addClass('fa-circle-play');
            videoPlaying = false;
        }
    });

    $('video').on('ended',function(){
        $('.videoContainer .mask').fadeIn();
        $('.videoContainer i').removeClass('fa-circle-pause').addClass('fa-circle-play');
        videoPlaying = false;
    });
});