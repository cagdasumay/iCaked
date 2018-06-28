/*
 * Plugin: Web Slide - Responsive Mega Menu for Bootstrap 3+
 * Demo Link: http://webslidemenu.webthemex.com/
 * Version: v3.3
 * Author: webthemex
 * License: http://codecanyon.net/licenses/standard
*/

document.addEventListener("touchstart", function () { }, false);
$(function () {
    $('.wsdownmenu').append($('<a class="wsdownmenu-animated-arrow"><span></span></a>'));
    $('.wsdownmenu').append($('<div class="wsdownmenu-text">Kategoriler</div>'));

    $('.wsdownmenu-list > li').has('.wsdownmenu-submenu').prepend('<span class="wsdownmenu-click"><i class="wsdownmenu-arrow fa fa-angle-down"></i></span>');
    $('.wsdownmenu-submenu > li').has('ul').prepend('<span class="wsdownmenu-click02"><i class="wsdownmenu-arrow fa fa-angle-down"></i></span>');
    $('.wsdownmenu-submenu-sub > li').has('ul').prepend('<span class="wsdownmenu-click02"><i class="wsdownmenu-arrow fa fa-angle-down"></i></span>');
    $('.wsdownmenu-submenu-sub-sub > li').has('ul').prepend('<span class="wsdownmenu-click02"><i class="wsdownmenu-arrow fa fa-angle-down"></i></span>');
    $('.wsdownmenu-list li').has('.megamenu').prepend('<span class="wsdownmenu-click"><i class="wsdownmenu-arrow fa fa-angle-down"></i></span>');

    $('.wsdownmenu-animated-arrow').click(function () {
        $('.wsdownmenu-list').slideToggle(300)
        $(this).toggleClass('wsdownmenu-lines');
    });

    $('.hasdd').on("mouseover", function (event) {
        event.preventDefault();
        var dis = this;
        $(".bakery-dd").not($("#" + $(this).data('dd'))).fadeOut(0,function () {
            $("#" + $(dis).data('dd')).fadeIn(0);
        });

        $(".bp-dd").not($("#" + $(this).data('bp'))).fadeOut(0, function () {
            $("#" + $(dis).data('bp')).fadeIn(0);
        });

        $(".hasdd a").css({ "background": "none" }); $(".hasdd span").css({ "margin-left": "0px" });
        $(this).find("a").css({ "background": "#f5f5f5" }); $(this).find("span").css({ "margin-left": "15px" });
        $(".hasbp a").css({ "background": "none" }); $(".hasbp span").css({ "margin-left": "0px" });

        $("#navbar-img-div").fadeIn(100);
    });

    //$('.hasbp').on("mouseover", function () {
    //    $(".bp-dd").not($("#" + $(this).data('bp'))).hide();
    //    $("#" + $(this).data('bp')).fadeIn(200);

    //    $(".hasbp a").css({ "background": "none" }); $(".hasbp span").css({ "margin-left": "0px" });
    //    $(this).find("a").css({ "background": "#f5f5f5" }); $(this).find("span").css({ "margin-left": "15px" });
    //});

    $('.wsdownmenu-click').click(function () {
        $(this).toggleClass('wsdownmenuarrow-rotate').parent().siblings().children().removeClass('wsdownmenuarrow-rotate');
        $(".wsdownmenu-submenu, .megamenu").not($(this).siblings('.wsdownmenu-submenu, .megamenu')).slideUp(300);
        $(this).siblings('.wsdownmenu-submenu').slideToggle(300);
        $(this).siblings('.megamenu').slideToggle(300);
    });

    $('.wsdownmenu-click02').click(function () {
        $(this).toggleClass('wsdownmenuarrow-rotate').parent().siblings().children().removeClass('wsdownmenuarrow-rotate');
        $(this).siblings('.wsdownmenu-submenu').slideToggle(300);
        $(this).siblings('.wsdownmenu-submenu-sub').slideToggle(300);
        $(this).siblings('.wsdownmenu-submenu-sub-sub').slideToggle(300);
    });

    // Remove inline styles when browser > 767
    window.onresize = function (event) {
        console.log('window resize');
        if ($(window).width() > 767) {
            $('.wsdownmenu-submenu').removeAttr("style");
            $('.wsdownmenu-list').removeAttr("style");
        }
    };
});