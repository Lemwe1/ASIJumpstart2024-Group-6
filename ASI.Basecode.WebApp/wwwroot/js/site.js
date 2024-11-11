document.addEventListener('DOMContentLoaded', () => {
    const toggleButton = document.getElementById('theme-toggle');

    if (toggleButton) {
        toggleButton.addEventListener('click', () => {
            // Toggle the dark mode class and save the preference in localStorage
            if (document.documentElement.classList.contains('dark')) {
                document.documentElement.classList.remove('dark');
                localStorage.setItem('theme', 'light');
            } else {
                document.documentElement.classList.add('dark');
                localStorage.setItem('theme', 'dark');
            }

            // Reload the page to apply the theme change immediately
            location.reload();
        });
    }
});
