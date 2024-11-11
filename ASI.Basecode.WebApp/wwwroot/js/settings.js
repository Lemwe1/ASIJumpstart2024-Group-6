document.addEventListener("DOMContentLoaded", () => {
    // Check if 'dark' theme is saved in localStorage or applied on body
    const savedTheme = localStorage.getItem('theme') || 'light';
    if (savedTheme === 'dark') {
        document.documentElement.classList.add('dark');
    } else {
        document.documentElement.classList.remove('dark');
    }

    // Listen for theme toggle (if you have a button or mechanism to change themes)
    const toggleButton = document.getElementById('theme-toggle');
    if (toggleButton) {
        toggleButton.addEventListener('click', () => {
            if (document.documentElement.classList.contains('dark')) {
                document.documentElement.classList.remove('dark');
                localStorage.setItem('theme', 'light');
            } else {
                document.documentElement.classList.add('dark');
                localStorage.setItem('theme', 'dark');
            }
        });
    }

    // Function to toggle password visibility
    function togglePasswordVisibility(inputId, iconId) {
        const passwordField = document.getElementById(inputId);
        const icon = document.getElementById(iconId);

        icon.addEventListener('click', function () {
            // Toggle the type between password and text
            if (passwordField.type === "password") {
                passwordField.type = "text"; // Show the password
                icon.classList.remove('fa-eye');
                icon.classList.add('fa-eye-slash'); // Change the icon to eye-slash
            } else {
                passwordField.type = "password"; // Hide the password
                icon.classList.remove('fa-eye-slash');
                icon.classList.add('fa-eye'); // Change the icon to eye
            }
        });
    }

    // Call the function for each password input field
    togglePasswordVisibility('current-password', 'toggle-current-password');
    togglePasswordVisibility('new-password', 'toggle-new-password');
    togglePasswordVisibility('confirm-password', 'toggle-confirm-password');
});