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

    // Profile picture change functionality
    const changePictureButton = document.getElementById('change-picture-btn');
    const fileInput = document.getElementById('file-input');
    const editProfilePicture = document.getElementById('edit-profile-picture');
    changePictureButton.addEventListener('click', () => {
        fileInput.click();  // Trigger the file input dialog
    });
    // Update profile picture preview when a new image is selected
    fileInput.addEventListener('change', (event) => {
        const file = event.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = (e) => {
                editProfilePicture.src = e.target.result;  // Set new image source
            };
            reader.readAsDataURL(file);
        }
    });

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

    // Modal for editing profile
    const modal = document.getElementById("editProfileModal");
    const openModalButton = document.getElementById("edit-profile-btn");
    const closeModalButton = document.getElementById("closeEditProfileModal");
    const cancelEditButton = document.getElementById("cancelEditProfileButton");

    // Function to open the profile edit modal
    function openEditProfileModal() {
        modal.classList.remove("hidden"); // Show the modal
        modal.classList.add("flex"); // Flex to center it
        document.body.classList.add('overflow-hidden');
    }

    // Function to close the profile edit modal
    function closeEditProfileModal() {
        modal.classList.add("hidden"); // Hide the modal
        modal.classList.remove("flex");
        document.body.classList.remove('overflow-hidden');
    }

    // Event listeners for opening and closing the profile edit modal
    openModalButton.addEventListener("click", openEditProfileModal);
    closeModalButton.addEventListener("click", closeEditProfileModal);
    cancelEditButton.addEventListener("click", closeEditProfileModal);

    // Optionally, close modal if user clicks outside the modal content area
    window.addEventListener("click", (e) => {
        if (e.target === modal) {
            closeEditProfileModal();
        }
    });

    // Modal for changing password
    const changePasswordModal = document.getElementById("changePasswordModal");
    const openChangePasswordModalButton = document.getElementById("change-password-btn");
    const closeChangePasswordModalButton = document.getElementById("closeChangePasswordModal");

    // Function to open the change password modal
    function openChangePasswordModal() {
        changePasswordModal.classList.remove("hidden");
        changePasswordModal.classList.add("flex");
        document.body.classList.add('overflow-hidden');
    }

    // Function to close the change password modal
    function closeChangePasswordModal() {
        changePasswordModal.classList.add("hidden");
        changePasswordModal.classList.remove("flex");
        document.body.classList.remove('overflow-hidden');
    }

    // Event listeners for opening and closing the password change modal
    openChangePasswordModalButton.addEventListener("click", openChangePasswordModal);
    closeChangePasswordModalButton.addEventListener("click", closeChangePasswordModal);

    // Optionally, close modal if user clicks outside the modal content area
    window.addEventListener("click", (e) => {
        if (e.target === changePasswordModal) {
            closeChangePasswordModal();
        }
    });
});
