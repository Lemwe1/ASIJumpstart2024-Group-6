document.addEventListener("DOMContentLoaded", () => {
    
    // Function to toggle password visibility
    function togglePasswordVisibility(inputId, iconId) {
        const passwordField = document.getElementById(inputId);
        const icon = document.getElementById(iconId);

        icon.addEventListener('click', function () {
            // Toggle between password and text type
            if (passwordField.type === "password") {
                passwordField.type = "text";
                icon.classList.remove('fa-eye-slash');
                icon.classList.add('fa-eye');
            } else {
                passwordField.type = "password";
                icon.classList.remove('fa-eye');
                icon.classList.add('fa-eye-slash');
            }
        });
    }

    // Call the togglePasswordVisibility function for each password input
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
