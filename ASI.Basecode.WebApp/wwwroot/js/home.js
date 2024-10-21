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
});