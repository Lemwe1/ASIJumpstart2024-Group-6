// Function to open the Create modal
function openCreateModal() {
    const modal = document.getElementById('createCategoryModal');
    modal.classList.remove('hidden');
    modal.classList.add('flex');
    // Set default type to Expense
    setTypeSelection('create', 'Expense');
}

// Close the Create modal
document.getElementById('closeCreateModal').addEventListener('click', function () {
    const modal = document.getElementById('createCategoryModal');
    modal.classList.add('hidden');
    modal.classList.remove('flex');
});

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
        } else {
            Swal.fire('Error', 'Failed to load category data.', 'error');
        }
    } catch (error) {
        console.error('Error loading category data:', error);
        Swal.fire('Error', 'Error occurred while loading the category data.', 'error');
    }
}

// Close the Edit modal
document.getElementById('closeEditModal').addEventListener('click', function () {
    const modal = document.getElementById('editCategoryModal');
    modal.classList.add('hidden');
    modal.classList.remove('flex');
});

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
