// Flag to track if changes were made
let isModalDirty = false;
const modal = document.getElementById('createCategoryModal');
const iconSelect = document.getElementById('createIcon');
const nameInput = document.getElementById('createName');
const colorInput = document.getElementById('createColorPicker');
// Function to toggle dark mode
function toggleDarkMode() {
    document.documentElement.classList.toggle('dark');
}

// Function to open the Create modal
function openCreateModal() {
    const modal = document.getElementById('createCategoryModal');
    const iconSelect = document.getElementById('createIcon');

    modal.classList.remove('hidden');
    modal.classList.add('flex');
    document.body.classList.add('overflow-hidden');  // Add overflow-hidden when modal opens

    // Reset the type selection and dirty flag
    setTypeSelection('create', 'Expense');
    isModalDirty = false; // Reset dirty flag when opening modal

    // Reset the icon select value
    iconSelect.value = ""; 
}


// Close the Create modal with confirmation if dirty
// Close the Create modal with confirmation if dirty
function closeCreateModal() {
    const modal = document.getElementById('createCategoryModal');
    if (isModalDirty) {
        Swal.fire({
            title: 'You have unsaved changes.',
            text: "Do you want to discard them?",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#a6a6a6',
            confirmButtonText: 'Discard',
            cancelButtonText: 'Cancel',
            customClass: { popup: 'swal2-front' }
        }).then((result) => {
            if (result.isConfirmed) {
                // Reset the fields
                iconSelect.value = "";
                nameInput.value = "";
                colorInput.value = "";

                modal.classList.add('hidden');
                modal.classList.remove('flex');
                document.body.classList.remove('overflow-hidden'); // Remove overflow-hidden when modal is closed
                isModalDirty = false; // Reset dirty flag
            }
        });
    } else {
        // Reset the fields if there were no changes
        iconSelect.value = "";
        nameInput.value = "";
        colorInput.value = "";

        modal.classList.add('hidden');
        modal.classList.remove('flex');
        document.body.classList.remove('overflow-hidden'); // Ensure overflow is removed if modal is closed without changes
    }
}


document.getElementById('resetCategoryButton').addEventListener('click', function () {
    
    iconSelect.value = "";
    nameInput.value = "";
    colorInput.value = "";
});


// Function to open the Edit modal
async function openEditModal(categoryId) {
    try {
        const response = await fetch(`/Category/GetCategory/${categoryId}`);
        if (response.ok) {
            const category = await response.json();
            document.body.classList.add('overflow-hidden');
            // Populate the edit form fields with category data
            document.getElementById('editId').value = category.id;
            document.getElementById('editName').value = category.name;
            document.getElementById('editIcon').value = category.icon;
            document.getElementById('editColorPicker').value = category.color;

            // Set the Type selection
            setTypeSelection('edit', category.type);

            // Show the edit modal
            const modal = document.getElementById('editCategoryModal');
            modal.classList.remove('hidden');
            modal.classList.add('flex');
             
            isModalDirty = false; // Reset dirty flag when opening modal
        } else {
            Swal.fire('Error', 'Failed to load category data.', 'error');
        }
    } catch (error) {
        console.error('Error loading category data:', error);
        Swal.fire('Error', 'Error occurred while loading the category data.', 'error');
    }
}

// Close the Edit modal with confirmation if dirty
function closeEditModal() {
    const modal = document.getElementById('editCategoryModal');

    if (isModalDirty) {
        Swal.fire({
            title: 'You have unsaved changes.',
            text: "Do you want to discard them?",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#a6a6a6',
            confirmButtonText: 'Discard',
            cancelButtonText: 'Cancel',
            customClass: { popup: 'swal2-front' }
        }).then((result) => {
            if (result.isConfirmed) {
                modal.classList.add('hidden');
                modal.classList.remove('flex');
                document.body.classList.remove('overflow-hidden'); // Remove overflow-hidden when modal is closed
                isModalDirty = false; // Reset dirty flag
            }
        });
    } else {
        modal.classList.add('hidden');
        modal.classList.remove('flex');
        document.body.classList.remove('overflow-hidden'); // Ensure overflow is removed if modal is closed without changes
    }
}


// Handle form field input to track dirty state
document.querySelectorAll('#createCategoryModal input, #editCategoryModal input').forEach(input => {
    input.addEventListener('input', () => {
        isModalDirty = true; // Set dirty flag when form input changes
    });
});

// Handle outside click to close modals
window.addEventListener('click', (e) => {
    const createModal = document.getElementById('createCategoryModal');
    const editModal = document.getElementById('editCategoryModal');

    if (e.target === createModal) {
        closeCreateModal();
    }
    if (e.target === editModal) {
        closeEditModal();
    }
});

// Close modal button event listeners
document.getElementById('closeCreateModal').addEventListener('click', closeCreateModal);
document.getElementById('closeEditModal').addEventListener('click', closeEditModal);

// Handle type selection for Create Modal
document.getElementById('createExpenseButton').addEventListener('click', function () {
    setTypeSelection('create', 'Expense');
});

document.getElementById('createIncomeButton').addEventListener('click', function () {
    setTypeSelection('create', 'Income');
});

// Handle type selection for Edit Modal
document.getElementById('editExpenseButton').addEventListener('click', function () {
    setTypeSelection('edit', 'Expense');
});

document.getElementById('editIncomeButton').addEventListener('click', function () {
    setTypeSelection('edit', 'Income');
});
