// Flag to track if changes were made
let isModalDirty = false;

// Function to open the Create modal
function openCreateModal() {
    const modal = document.getElementById('createCategoryModal');
    modal.classList.remove('hidden');
    modal.classList.add('flex');
    setTypeSelection('create', 'Expense');
    isModalDirty = false; // Reset dirty flag when opening modal
}

// Close the Create modal with confirmation if dirty
function closeCreateModal() {
    const modal = document.getElementById('createCategoryModal');
    if (isModalDirty) {
        if (confirm("You have unsaved changes. Do you want to discard them?")) {
            modal.classList.add('hidden');
            modal.classList.remove('flex');
            isModalDirty = false; // Reset flag
        }
    } else {
        modal.classList.add('hidden');
        modal.classList.remove('flex');
    }
}

// Function to open the Edit modal
async function openEditModal(categoryId) {
    try {
        const response = await fetch(`/Category/GetCategory/${categoryId}`);
        if (response.ok) {
            const category = await response.json();

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
        if (confirm("You have unsaved changes. Do you want to discard them?")) {
            modal.classList.add('hidden');
            modal.classList.remove('flex');
            isModalDirty = false; // Reset flag
        }
    } else {
        modal.classList.add('hidden');
        modal.classList.remove('flex');
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
git