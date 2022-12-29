/*const { debug } = require("console");*/
function SaveUsers() {
    debugger
    var name = $("#name").val();
    var email = $("#email").val();
    var pass = $("#password").val();
    $.ajax({
        type: "POST",
        url: "/Home/Register",
        data: {name:name,email:email,password:pass},
        success: function (response) {
            debugger
            if (response==true) {
                $("#name").val("");
                $("#name").val("");
                $("#password").val("");
                $(document).ready(function () {
                    $("#loadlogin").click();
                    $("#loginemail").val(email);
                    $("#loginpassword").val(pass);
                });
            }
            else {
                alert(response);
            }
        },
        error: function (response) {
            debugger    
            alert(response);
        }
    })
}

function LoginUsers() {
    //debugger
    var loginEmail = $("#loginemail").val();
    var loginPassword = $("#loginpassword").val();
    $.ajax({
        type: "POST",
        url: "/Home/Login",
        data: { email: loginEmail, password: loginPassword },
        success: function (response) {
             //debugger
            if (response == true) {
                window.location.replace("/");
            }
            else {
                alert(response);
                
            }
        }
    })
}
function ChangePassword() {
    //debugger
    var nemail = $("#CPE").val();
    var npass = $("#NP").val();
    var cnpass = $("#CNP").val();
    if (npass == cnpass) {
        $.ajax({
            type: "POST",
            url: "/Home/ForgetPassword",
            data: { email: nemail, password: npass},
            success: function (response) {
                debugger
                if (response == false) {
                    $("#check").text("Wrong Email! ");
                }
                else {
                    $("#success").text(response);
                    $("#CPE").val("");
                    $("#NP").val("");
                    $("#CNP").val("");
                }
            },
            error: function (response) {
                alert(response);
            }
        })
    }
    else {
        $("#check").text("Confirm New Password Doesn't Match");
    }
}

//function MyUrls(model) {
//    debugger
//    $.ajax({
//        type: "POST",
//        url: "/Home/MyUrls",
//        data: { model: model },
//        success: function () {
//            window.localStorage.href = "/Home/MyUrls";
//        }
//    })



function RemoveAccount(id) {
    //debugger
    $.ajax({
        type: "POST",
        url: "/Home/RemoveUser",
        data: { id: id },
        success: function () {
            window.location.href = "/Home/Logout";
        }
    })
}
