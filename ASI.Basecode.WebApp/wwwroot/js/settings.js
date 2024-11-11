document.addEventListener("DOMContentLoaded", () => {

    // Function to toggle password visibility and persist state
    function togglePasswordVisibility(inputId, iconId) {
        const passwordField = document.getElementById(inputId);
        const icon = document.getElementById(iconId);
        const visibilityKey = `${inputId}_visibility`;

        // Set initial visibility based on localStorage
        const isPasswordVisible = localStorage.getItem(visibilityKey) === 'true';
        updatePasswordVisibility(passwordField, icon, isPasswordVisible);

        icon.addEventListener('click', function () {
            // Toggle between password and text type
            const isVisible = passwordField.type === "text";
            updatePasswordVisibility(passwordField, icon, !isVisible);
            localStorage.setItem(visibilityKey, !isVisible); // Save the new visibility state
        });
    }

    // Helper function to update password field type and icon
    function updatePasswordVisibility(passwordField, icon, isVisible) {
        if (isVisible) {
            passwordField.type = "text";
            icon.classList.remove('fa-eye-slash');
            icon.classList.add('fa-eye');
        } else {
            passwordField.type = "password";
            icon.classList.remove('fa-eye');
            icon.classList.add('fa-eye-slash');
        }
    }

    // Call the function for each password input field
    togglePasswordVisibility('current-password', 'toggle-current-password');
    togglePasswordVisibility('new-password', 'toggle-new-password');
    togglePasswordVisibility('confirm-password', 'toggle-confirm-password');


    // Profile picture change functionality
    const changePictureButton = document.getElementById('change-picture-btn');
    const fileInput = document.getElementById('file-input');
    const profilePicture = document.getElementById('profile-picture');
    changePictureButton.addEventListener('click', () => {
        fileInput.click();  // Trigger the file input dialog
    });
    // Update profile picture preview when a new image is selected
    fileInput.addEventListener('change', (event) => {
        const file = event.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = (e) => {
                profilePicture.src = e.target.result;  // Set new image source
            };
            reader.readAsDataURL(file);
        }
    });
});