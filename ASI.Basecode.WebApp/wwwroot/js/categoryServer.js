﻿// categoryServer.js

// Function to open the Edit modal
async function openEditModal(categoryId) {
    console.log('Opening edit modal for category ID:', categoryId);

    try {
        const response = await fetch(`/Category/GetCategory/${categoryId}`);
        console.log('Response from fetching category data:', response);

        if (response.ok) {
            const result = await response.json();
            console.log('Category data:', result);

            if (result.success && result.data) {
                const category = result.data;
                console.log('Extracted category:', category);

                // Log available properties
                console.log('Available properties:', Object.keys(category));

                // Check if 'type' exists
                if (!category.hasOwnProperty('type')) {
                    console.error('Category data does not contain "type" property.');
                    Swal.fire('Error', 'Category data is missing the "Type" property.', 'error');
                    return;
                }

                // Populate the edit form fields with category data
                document.getElementById('editId').value = category.categoryId || category.CategoryId;
                console.log('Set editId:', category.categoryId || category.CategoryId);

                document.getElementById('editName').value = category.name;
                console.log('Set editName:', category.name);

                document.getElementById('editIcon').value = category.icon;
                console.log('Set editIcon:', category.icon);

                document.getElementById('editColorPicker').value = category.color;
                console.log('Set editColorPicker:', category.color);

                // Set the Type selection
                setTypeSelection('edit', category.type);
                console.log('Set Type selection to:', category.type);

                // Hide the opposite type button in the edit modal
                const expenseButton = document.getElementById('editExpenseButton');
                const incomeButton = document.getElementById('editIncomeButton');

                if (category.type === 'Expense') {
                    incomeButton.classList.add('hidden');
                } else if (category.type === 'Income') {
                    expenseButton.classList.add('hidden');
                }
                console.log('Adjusted type buttons based on category type.');

                // Disable the visible type button to prevent changing type
                const typeButton = category.type === 'Expense' ? expenseButton : incomeButton;
                typeButton.disabled = true;
                typeButton.classList.add('opacity-50', 'cursor-not-allowed');

                // Show the edit modal
                const modal = document.getElementById('editCategoryModal');
                modal.classList.remove('hidden');
                modal.classList.add('flex');
                console.log('Edit modal displayed.');

                // Hide delete button if category is "Default Income" or "Default Expense"
                const deleteButton = document.getElementById('deleteButton');
                if (category.name === 'Default Income' || category.name === 'Default Expense') {
                    deleteButton.classList.add('hidden');
                } else {
                    deleteButton.classList.remove('hidden');
                }
            } else {
                Swal.fire('Error', 'Failed to load category data.', 'error');
            }
        } else {
            Swal.fire('Error', 'Failed to load category data.', 'error');
        }
    } catch (error) {
        console.error('Error loading category data:', error);
        Swal.fire('Error', 'Error occurred while loading the category data.', 'error');
    }
}

// Handle form submission for creating a new category
document.getElementById('categoryForm').addEventListener('submit', async function (event) {
    event.preventDefault();

    const formData = new FormData(this);

    try {
        const response = await fetch('/Category/Create', {
            method: 'POST',
            body: formData
        });

        if (response.ok) {
            const result = await response.json();
            if (result.success) {
                Swal.fire('Success', 'Category created successfully!', 'success').then(() => {
                    window.location.reload();
                });
            } else {
                Swal.fire('Error', result.message || 'Failed to create category.', 'error');
            }
        } else {
            const result = await response.json();
            Swal.fire('Error', result.message || 'Failed to create category.', 'error');
        }
    } catch (error) {
        Swal.fire('Error', 'An error occurred while creating the category.', 'error');
    }
});

// Handle form submission for editing a category
document.getElementById('editCategoryForm').addEventListener('submit', async function (event) {
    event.preventDefault();
    console.log('Submitting edit category form');

    const formData = new FormData(this);
    console.log('Form data:', [...formData]);

    try {
        const id = formData.get('CategoryId'); // Must match the hidden input's name
        console.log('Editing category ID:', id);

        if (!id) {
            Swal.fire('Error', 'Category ID is missing.', 'error');
            return;
        }

        const response = await fetch(`/Category/Edit/${id}`, {
            method: 'POST',
            body: formData
        });
        console.log('Response from edit category:', response);

        if (response.ok) {
            const result = await response.json();
            if (result.success) {
                const modal = document.getElementById('editCategoryModal');
                modal.classList.add('hidden');
                modal.classList.remove('flex');

                Swal.fire('Success', 'Category updated successfully!', 'success').then(() => {
                    window.location.reload();
                });
            } else {
                Swal.fire('Error', result.message || 'Failed to update category.', 'error');
            }
        } else {
            const result = await response.json();
            Swal.fire('Error', result.message || 'Failed to update category.', 'error');
        }
    } catch (error) {
        console.error('Error updating category:', error);
        Swal.fire('Error', 'Error occurred while updating the category.', 'error');
    }
});

// Handle category deletion
document.getElementById('deleteButton').addEventListener('click', async function () {
    const id = document.getElementById('editId').value;
    console.log('Attempting to delete category with ID:', id);

    if (!id) {
        Swal.fire('Error', 'Category ID is missing.', 'error');
        return;
    }

    Swal.fire({
        title: 'Are you sure?',
        text: 'You won\'t be able to revert this!',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#e53e3e',
        cancelButtonColor: '#718096',
        confirmButtonText: 'Yes, delete it!'
    }).then(async (result) => {
        if (result.isConfirmed) {
            const formData = new FormData();
            formData.append('__RequestVerificationToken', document.querySelector('#editCategoryForm input[name="__RequestVerificationToken"]').value);
            console.log('Form data for deletion:', [...formData]);

            try {
                const response = await fetch(`/Category/Delete/${id}`, {
                    method: 'POST',
                    body: formData
                });
                console.log('Response from delete category:', response);

                if (response.ok) {
                    const result = await response.json();
                    if (result.success) {
                        const modal = document.getElementById('editCategoryModal');
                        modal.classList.add('hidden');
                        modal.classList.remove('flex');

                        Swal.fire('Deleted!', 'Category has been deleted.', 'success').then(() => {
                            window.location.reload();
                        });
                    } else {
                        Swal.fire('Error', result.message || 'Failed to delete category.', 'error');
                    }
                } else {
                    const result = await response.json();
                    Swal.fire('Error', result.message || 'Failed to delete category.', 'error');
                }
            } catch (error) {
                console.error('Error deleting category:', error);
                Swal.fire('Error', 'Error occurred while deleting the category.', 'error');
            }
        }
    });
});

// Function to handle type selection and highlight the selected button
function setTypeSelection(modalType, selectedType) {
    console.log('Setting type selection for:', modalType, 'Type:', selectedType);

    let expenseButton, incomeButton, typeInput;

    if (modalType === 'create') {
        expenseButton = document.getElementById('createExpenseButton');
        incomeButton = document.getElementById('createIncomeButton');
        typeInput = document.getElementById('createTypeInput');
    } else if (modalType === 'edit') {
        expenseButton = document.getElementById('editExpenseButton');
        incomeButton = document.getElementById('editIncomeButton');
        typeInput = document.getElementById('editTypeInput');
    }

    typeInput.value = selectedType;
    console.log(`Type input set to: ${selectedType}`);

    // Reset both buttons to default styles first
    expenseButton.classList.remove('bg-red-400', 'text-white', 'opacity-50', 'cursor-not-allowed', 'hidden');
    expenseButton.classList.add('bg-gray-200', 'text-gray-800');
    expenseButton.disabled = false;

    incomeButton.classList.remove('bg-green-500', 'text-white', 'opacity-50', 'cursor-not-allowed', 'hidden');
    incomeButton.classList.add('bg-gray-200', 'text-gray-800');
    incomeButton.disabled = false;

    // Then apply selected styles
    if (selectedType === 'Expense') {
        expenseButton.classList.add('bg-red-400', 'text-white');
    } else if (selectedType === 'Income') {
        incomeButton.classList.add('bg-green-500', 'text-white');
    }
}

// Event listeners for type buttons in the create modal only
document.getElementById('createExpenseButton').addEventListener('click', function () {
    setTypeSelection('create', 'Expense');
});

document.getElementById('createIncomeButton').addEventListener('click', function () {
    setTypeSelection('create', 'Income');
});

// Event listener to close the edit modal
document.getElementById('closeEditModal').addEventListener('click', function () {
    const modal = document.getElementById('editCategoryModal');
    modal.classList.add('hidden');
    modal.classList.remove('flex');

    // Reset the disabled state and visibility of type buttons
    const expenseButton = document.getElementById('editExpenseButton');
    const incomeButton = document.getElementById('editIncomeButton');

    expenseButton.disabled = false;
    incomeButton.disabled = false;

    expenseButton.classList.remove('opacity-50', 'cursor-not-allowed', 'hidden');
    incomeButton.classList.remove('opacity-50', 'cursor-not-allowed', 'hidden');
});
