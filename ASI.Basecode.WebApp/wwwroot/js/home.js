document.addEventListener('DOMContentLoaded', () => {

    const fadeOutDialog = (dialog, duration) => {
        setTimeout(() => {
            dialog.classList.remove('opacity-100'); // Fade out
            dialog.classList.add('opacity-0'); // Make it invisible
        }, duration);
    };

    const initializeDialog = () => {
        const dialog = document.getElementById('dialog-message');

        if (dialog) {
            dialog.classList.remove('opacity-0'); 
            dialog.classList.add('opacity-100'); // Fade in

            setTimeout(() => {
                fadeOutDialog(dialog, 3000); 
            }, 3000); // visible for 3 seconds
        }
    };

    initializeDialog();

    // Check if 'dark' theme is saved in localStorage or applied on body
    const savedTheme = localStorage.getItem('theme') || 'light';
    if (savedTheme === 'dark' || document.body.classList.contains('dark')) {
        document.body.classList.add('dark');
        localStorage.setItem('theme', 'dark');
    } else {
        document.body.classList.remove('dark');
        localStorage.setItem('theme', 'light');
    }
});