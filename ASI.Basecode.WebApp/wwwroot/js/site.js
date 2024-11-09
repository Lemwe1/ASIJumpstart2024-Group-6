// Logout confirmation dialog
document.getElementById('logoutLink').addEventListener('click', function (event) {
    event.preventDefault();

    Swal.fire({
        text: "Are you sure you want to log out?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#a6a6a6',
        confirmButtonText: 'Logout'
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = this.getAttribute('href');
        }
    });
});