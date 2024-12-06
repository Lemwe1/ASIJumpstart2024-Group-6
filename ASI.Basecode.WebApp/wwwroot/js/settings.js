document.addEventListener("DOMContentLoaded", () => {
    // Existing functionality
    function togglePasswordVisibility(inputId, iconId) {
        const passwordField = document.getElementById(inputId);
        const icon = document.getElementById(iconId);
        const visibilityKey = `${inputId}_visibility`;

        const isPasswordVisible = localStorage.getItem(visibilityKey) === 'true';
        updatePasswordVisibility(passwordField, icon, isPasswordVisible);

        icon.addEventListener('click', function () {
            const isVisible = passwordField.type === "text";
            updatePasswordVisibility(passwordField, icon, !isVisible);
            localStorage.setItem(visibilityKey, !isVisible);
        });
    }

    const changePictureButton = document.getElementById('change-picture-btn');
    const fileInput = document.getElementById('file-input');
    const editProfilePicture = document.getElementById('edit-profile-picture');
    changePictureButton.addEventListener('click', () => {
        fileInput.click();
    });
    fileInput.addEventListener('change', (event) => {
        const file = event.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = (e) => {
                editProfilePicture.src = e.target.result;
            };
            reader.readAsDataURL(file);
        }
    });

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

    togglePasswordVisibility('current-password', 'toggle-current-password');
    togglePasswordVisibility('new-password', 'toggle-new-password');
    togglePasswordVisibility('confirm-password', 'toggle-confirm-password');

    const modal = document.getElementById("editProfileModal");
    const openModalButton = document.getElementById("edit-profile-btn");
    const closeModalButton = document.getElementById("closeEditProfileModal");
    const cancelEditButton = document.getElementById("cancelEditProfileButton");
    const editProfileForm = document.getElementById("editProfileForm");

    function openEditProfileModal() {
        modal.classList.remove("hidden");
        modal.classList.add("flex");
        document.body.classList.add('overflow-hidden');
    }

    function closeEditProfileModal() {
        modal.classList.add("hidden");
        modal.classList.remove("flex");
        document.body.classList.remove('overflow-hidden');
    }

    openModalButton.addEventListener("click", openEditProfileModal);
    closeModalButton.addEventListener("click", closeEditProfileModal);
    cancelEditButton.addEventListener("click", closeEditProfileModal);

    window.addEventListener("click", (e) => {
        if (e.target === modal) {
            closeEditProfileModal();
        }
    });

    // Updated submission handler for EditProfile form with confirmation modal
    // Submission handler for the EditProfile form
    const confirmEditButton = document.getElementById("confirmEditButton"); // Replace with the actual ID of your confirm button

    if (confirmEditButton) {
        confirmEditButton.addEventListener("click", function (e) {
            e.preventDefault(); // Prevent the form from being submitted immediately

            Swal.fire({
                title: 'Confirm Changes',
                text: 'Do you want to save the changes to your profile?',
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Save',
                cancelButtonText: 'Cancel'
            }).then((result) => {
                if (result.isConfirmed) {
                    // Submit the form only after confirmation
                    editProfileForm.submit();
                } else {
                    Swal.fire({
                        title: 'Cancelled',
                        text: 'Your changes were not saved.',
                        icon: 'info',
                        customClass: { popup: 'swal2-front' }
                    });
                }
            });
        });
    }

    const changePasswordModal = document.getElementById("changePasswordModal");
    const openChangePasswordModalButton = document.getElementById("change-password-btn");
    const closeChangePasswordModalButton = document.getElementById("closeChangePasswordModal");

    function openChangePasswordModal() {
        changePasswordModal.classList.remove("hidden");
        changePasswordModal.classList.add("flex");
        document.body.classList.add('overflow-hidden');
    }

    function closeChangePasswordModal() {
        changePasswordModal.classList.add("hidden");
        changePasswordModal.classList.remove("flex");
        document.body.classList.remove('overflow-hidden');
    }

    openChangePasswordModalButton.addEventListener("click", openChangePasswordModal);
    closeChangePasswordModalButton.addEventListener("click", closeChangePasswordModal);

    window.addEventListener("click", (e) => {
        if (e.target === changePasswordModal) {
            closeChangePasswordModal();
        }
    });
});
