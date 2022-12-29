const { text } = require("node:stream/consumers");
const { debug } = require("node:util");

/*const { debug } = require("console");*/
$(document).ready(function() {
    $('#buttons').hide();
});
function SaveURLMapping() {
    debugger
    var url = $('#mapUrl').val();
    var alias = $('#customUrl').val();
    var userId = $("#userId").val();
    if (url != '') {
        $.ajax({
            type: "POST",
            url: "/Home/SaveURLMapping",
            data: { url: url, alias: alias, userId: userId },
            success: function (msg) {
                debugger
                if (msg != false) {
                    $('#mapUrl').val('');
                    $('#customUrl').val('');
                    $('#btnURL').hide();
                    $('#generatedText').val(msg);
                    $('#textbox').val(msg);
                    $('#generatedText').show();
                    $("#copyBtn").show();
                    $('#buttons').show();
                    $(".myurls-short").show();
                    toastr.success('Your url has been created'); //Displaying success message   
                }
                else
                    toastr.error('Alias URL already exists');//error message
                // alert(msg);
            }
        });
    }
    else {
        $("#invalidurl").text(" Type any URL to Convert");
        setTimeout(function () {
            $("#invalidurl").hide();
        }, 4000);       
    }
}

function CopyText() {
    debugger
    var copyText = document.getElementById("generatedText");

    copyText.select();
    copyText.setSelectionRange(0, 99999);

    navigator.clipboard.writeText(copyText.value);
}
function CommonCopyUrl() {
    /*debugger*/
    var r = document.createRange();
    r.selectNode(document.getElementById("generatedText"));
    window.getSelection().removeAllRanges();
    window.getSelection().addRange(r);
    document.execCommand('copy');
    window.getSelection().removeAllRanges();
}
function Whatsapp() {
    var url = $("#generatedText").val();
    CommonCopyUrl();
    window.open('https://web.whatsapp.com://send?text='+url);

}
function Facebook() {
    CommonCopyUrl();
    window.open('https://www.facebook.com');

}
function Twitter() {
    CommonCopyUrl();
    window.open('https://twitter.com');

}
function Google() {
    CommonCopyUrl();
    window.open('https://www.google.com');

}
function Linkedin() {
    CommonCopyUrl();
    window.open('https://in.linkedin.com');

}

function OpenGmail() {
    CommonCopyUrl();
    window.open('https://mail.google.com'); 
}


function Instagram() {
    CommonCopyUrl();
    window.open('https://www.instagram.com');

}
function ShareOnGoogle(url) {
    window.open(url);
}

function ShareUrl() {
    //debugger
    var newurl = $("#dropdownMenuButton").text();
    window.open("newurl");
}

function Copyurl() {
    debugger
    var r = document.createRange();
    r.selectNode(document.getElementById("lilliurl"));
    window.getSelection().removeAllRanges();
    window.getSelection().addRange(r);
    document.execCommand('copy');
    window.getSelection().removeAllRanges();
       
}

function RemoveUrl(Id) {
    if (Id != null) {
        $.ajax({
            type: "POST",
            url: "/Home/RemoveURL",
            data: { Id: Id },
            success: function (response) {
                window.location.href = "/";
            }
        })
    }
    else {
        return; 
    }
    
}
function anotherShort() {
    $('#mapUrl').val('');
    $('#customUrl').val('');
    $('#btnURL').show();
    $('#generatedText').hide();
    $("#copyBtn").hide();
    $('#buttons').hide();
    $(".myurls-short").hide();
}




