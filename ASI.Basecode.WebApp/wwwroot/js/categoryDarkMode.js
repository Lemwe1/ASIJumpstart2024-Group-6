// Dark Mode Toggle Functionality
const darkModeToggle = document.getElementById('darkModeToggle');
const darkModeIcon = document.getElementById('darkModeIcon');

// Check local storage for dark mode preference
if (localStorage.getItem('theme') === 'dark') {
    enableDarkMode();
}

darkModeToggle.addEventListener('click', () => {
    if (document.body.classList.contains('dark')) {
        disableDarkMode();
    } else {
        enableDarkMode();
    }
});

function enableDarkMode() {
    document.body.classList.add('dark');
    darkModeIcon.classList.remove('fa-moon');
    darkModeIcon.classList.add('fa-sun');
    localStorage.setItem('theme', 'dark');

    // Update background and text colors
    document.body.classList.add('bg-gray-900', 'text-gray-100');
    document.body.classList.remove('bg-white', 'text-gray-900');

    // Update category cards
    const categoryCards = document.querySelectorAll('.category-card');
    categoryCards.forEach(card => {
        card.classList.add('darken-category-card');
    });

    // Update modals
    const modals = document.querySelectorAll('.modal-content');
    modals.forEach(modal => {
        modal.classList.add('darken-modal-content');
    });
}

function disableDarkMode() {
    document.body.classList.remove('dark');
    darkModeIcon.classList.remove('fa-sun');
    darkModeIcon.classList.add('fa-moon');
    localStorage.setItem('theme', 'light');

    // Update background and text colors
    document.body.classList.add('bg-white', 'text-gray-900');
    document.body.classList.remove('bg-gray-900', 'text-gray-100');

    // Update category cards
    const categoryCards = document.querySelectorAll('.category-card');
    categoryCards.forEach(card => {
        card.classList.remove('darken-category-card');
    });

    // Update modals
    const modals = document.querySelectorAll('.modal-content');
    modals.forEach(modal => {
        modal.classList.remove('darken-modal-content');
    });
}
