document.addEventListener("DOMContentLoaded", () => {

    // Theme toggle logic
    const toggle = document.getElementById('theme-toggle');
    const sunIcon = document.getElementById('sun-icon');
    const moonIcon = document.getElementById('moon-icon');

    // Check if user has a previously saved theme preference
    const savedTheme = localStorage.getItem('theme') || 'light';
    if (savedTheme === 'dark') {
        document.body.classList.add('dark');
        sunIcon.classList.remove('hidden');
        moonIcon.classList.add('hidden');
    }

    // Toggle between dark and light mode
    toggle.addEventListener('click', () => {
        document.body.classList.toggle('dark');

        // Toggle icon visibility
        sunIcon.classList.toggle('hidden');
        moonIcon.classList.toggle('hidden');

        // Save the theme preference in local storage
        const theme = document.body.classList.contains('dark') ? 'dark' : 'light';
        localStorage.setItem('theme', theme);
    });

    // Logout confirmation dialog
    document.getElementById('miniLogoutLink').addEventListener('click', function (event) {
        event.preventDefault();

        Swal.fire({
            text: "Are you sure you want to log out?",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Logout',
            confirmButtonColor: darkMode ? '#d33' : '#3085d6', 
            cancelButtonColor: darkMode ? '#555' : '#a6a6a6', 
            customClass: {
                popup: darkMode ? 'bg-gray-900 text-white' : 'bg-white text-gray-900', 
            },
        }).then((result) => {
            if (result.isConfirmed) {
                window.location.href = this.getAttribute('href');
            }
        });
    });

    // Logout confirmation dialog
    document.getElementById('logoutLink').addEventListener('click', function (event) {
        event.preventDefault();

        Swal.fire({
            text: "Are you sure you want to log out?",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Logout',
            confirmButtonColor: darkMode ? '#d33' : '#3085d6',
            cancelButtonColor: darkMode ? '#555' : '#a6a6a6',
            customClass: {
                popup: darkMode ? 'bg-gray-900 text-white' : 'bg-white text-gray-900',
            },
        }).then((result) => {
            if (result.isConfirmed) {
                window.location.href = this.getAttribute('href');
            }
        });
    });
});