﻿document.addEventListener('DOMContentLoaded', function () {
    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', function (event) {
            event.preventDefault(); // Prevent form from submitting the usual way

            const overlay = document.getElementById('form-overlay');
            const spinner = document.getElementById('loading-spinner');
            const successIcon = document.getElementById('success-icon');
            const failureIcon = document.getElementById('failure-icon');
            const successMessageDiv = document.getElementById('success-message');
            const errorMessageDiv = document.getElementById('error-message');
            const submitButton = document.getElementById('submitButton');

            // Change the button text to "Submitting..."
            submitButton.textContent = "Logging in...";
            submitButton.disabled = true; // Disable the button to prevent multiple clicks

            // Reset all icons, spinner, and messages to initial hidden state
            spinner.classList.add('hidden');
            successIcon.classList.add('hidden');
            failureIcon.classList.add('hidden');
            successMessageDiv.classList.add('hidden');
            errorMessageDiv.classList.add('hidden'); // Hide the error message
            overlay.classList.remove('hidden'); // Show the overlay

            // Show loading spinner
            spinner.classList.remove('hidden');

            // Prepare the form data
            const formData = new FormData(loginForm);

            // Send an AJAX request to the login endpoint
            fetch('/Account/Login', {
                method: 'POST',
                body: formData,
            })
                .then(response => response.json())
                .then(data => {
                    // Hide loading spinner
                    spinner.classList.add('hidden');

                    if (data.success) {
                        successIcon.classList.remove('hidden');
                        successMessageDiv.textContent = data.message; // Show success message from the response
                        successMessageDiv.classList.remove('hidden'); // Display the success message

                        // Redirect after showing success message for 2 seconds
                        setTimeout(function () {
                            window.location.href = "/Home/Index"; // Redirect to home page
                        }, 2000);
                    } else {
                        failureIcon.classList.remove('hidden');

                        // Show error message from the response
                        if (data.resendVerification) {
                            errorMessageDiv.innerHTML = `${data.message} <a href='/Account/ResendVerificationLink' class='text-blue-600 hover:underline'>Resend Verification Email</a>`;
                        } else {
                            errorMessageDiv.textContent = data.message;
                        }

                        errorMessageDiv.classList.remove('hidden'); // Display the error message

                        // Reset the button text and state after a delay
                        setTimeout(function () {
                            overlay.classList.add('hidden');
                            failureIcon.classList.add('hidden');
                            submitButton.textContent = "Submit";
                            submitButton.disabled = false;
                        }, 2000);
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    spinner.classList.add('hidden');
                    failureIcon.classList.remove('hidden');

                    // Reset the button text and state after a delay
                    setTimeout(function () {
                        overlay.classList.add('hidden');
                        failureIcon.classList.add('hidden');
                        submitButton.textContent = "Submit";
                        submitButton.disabled = false;
                    }, 2000);
                });
        });

        const togglePassword = document.getElementById('toggle-password');
        const passwordField = document.getElementById('form2Example22');

        if (togglePassword && passwordField) {
            togglePassword.addEventListener('click', function () {
                // Toggle the type of the password field between password and text
                const type = passwordField.type === 'password' ? 'text' : 'password';
                passwordField.type = type;

                // Toggle the eye icon to show the correct status (open/closed)
                this.classList.toggle('fa-eye');
                this.classList.toggle('fa-eye-slash');
            });
        }
    }
});
