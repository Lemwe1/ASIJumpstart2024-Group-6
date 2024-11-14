﻿document.getElementById('registerForm').addEventListener('submit', function (event) {
    event.preventDefault(); // Prevent form from submitting the usual way

    const overlay = document.getElementById('form-overlay');
    const spinner = document.getElementById('loading-spinner');
    const successIcon = document.getElementById('success-icon');
    const failureIcon = document.getElementById('failure-icon');
    const successMessageDiv = document.getElementById('success-message');
    const errorMessageDiv = document.getElementById('error-message');

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
    const formData = new FormData(this);

    // Send an AJAX request to the register endpoint
    fetch('/Account/Register', {
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
                    window.location.href = "/Account/Login"; // Redirect to login page
                }, 2000);
            } else {
                failureIcon.classList.remove('hidden');
                errorMessageDiv.textContent = data.message; // Show error message from the response
                errorMessageDiv.classList.remove('hidden'); // Display the error message
                setTimeout(function () {
                    overlay.classList.add('hidden');
                    failureIcon.classList.add('hidden');
                }, 2000);
            }
        })
        .catch(error => {
            console.error('Error:', error);
            spinner.classList.add('hidden');
            failureIcon.classList.remove('hidden');
            setTimeout(function () {
                overlay.classList.add('hidden');
                failureIcon.classList.add('hidden');
            }, 2000);
        });
});

