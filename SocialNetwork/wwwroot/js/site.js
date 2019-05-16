/// <reference path="jquery.js" />
// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {

    $(".commentToggle").on("click", function () {
        var id = $(this).attr("name");
        var comments = $("#" + id);

        if ($(comments).hasClass("open")) {
            $(comments).removeClass("open");
            $(this).html("Show comments");
            
        }
        else {
            $(comments).addClass("open");
            $(this).html("Hide comments");
        }
    });

    var isChrome = /Chrome/.test(navigator.userAgent) && /Google Inc/.test(navigator.vendor);
    if (window.location.hash && isChrome) {
        console.log("is chrome");
        setTimeout(function () {
            var hash = window.location.hash;
            window.location.hash = "";
            window.location.hash = hash;
        }, 300);
    }
});

function closePopUp()
{
    $("#popupWrapper").fadeOut();
    $("#popup").fadeOut();
}

