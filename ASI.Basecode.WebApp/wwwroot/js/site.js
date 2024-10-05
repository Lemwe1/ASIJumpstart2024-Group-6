let dpicn = document.querySelector(".dpicn");
let dropdown = document.querySelector(".dropdown");

dpicn.addEventListener("click", () => {
    dropdown.classList.toggle("dropdown-open");
})

// Configure Toastr options
toastr.options = {
    positionClass: "toast-top-center",
    timeOut: 3000,
    closeButton: true,
    progressBar: true
};

