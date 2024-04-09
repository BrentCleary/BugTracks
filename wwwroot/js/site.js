// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function fillLoginForm() {

    console.log("FIll LOGIN FORM CLICKED __+_+_+_+_+_+_+_+_+_+")

    // Replace these values with your predetermined account information
    var email = "demoadmin@bugtracker.com";
    var password = "Abc&123!";

    // Find the email and password fields on the page
    var emailField = document.getElementById("emailFieldId"); // Replace "emailFieldId" with the actual ID of your email input field
    var passwordField = document.getElementById("passwordFieldId"); // Replace "passwordFieldId" with the actual ID of your password input field

    // Check if the fields exist
    if (emailField && passwordField) {
        // Fill in the email and password fields
        emailField.value = email;
        passwordField.value = password;
    } else {
        console.error("Email or password field not found.");
    }
}