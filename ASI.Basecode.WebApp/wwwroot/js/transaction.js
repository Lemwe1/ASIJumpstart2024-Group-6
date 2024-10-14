document.addEventListener("DOMContentLoaded", () => {
    const modal = document.getElementById('addTransactionModal');
    const addTransactionButton = document.getElementById('openAddTransactionModal');
    const closeModalButton = document.getElementById('closeTransactionModal');
    const transactionForm = document.getElementById('transactionForm');
    const createExpenseButton = document.getElementById('createExpenseButton');
    const createIncomeButton = document.getElementById('createIncomeButton');
    const transactionCategorySelect = document.getElementById('transactionCategory');
    const transactionAccountSelect = document.getElementById('transactionAccount');

    let transactionType = 'Expense'; // Default type
    let isModalDirty = false; // Flag to track if changes were made

    // Open modal function
    function openAddTransactionModal() {
        modal.classList.remove('hidden');
        modal.classList.add('flex');
        transactionForm.reset(); // Reset form when opening modal
        updateTypeSelection(transactionType); // Ensure the type button reflects the default selection
        filterCategories(transactionType); // Filter categories on modal open
        filterAccounts(transactionType); // Filter accounts on modal open
        isModalDirty = false; // Reset the dirty flag
    }

    // Close modal function with confirmation
    function closeAddTransactionModal() {
        if (isModalDirty) {
            if (confirm("You have unsaved changes. Do you want to discard them?")) {
                resetModal();
            }
        } else {
            resetModal();
        }
    }

    // Function to reset modal state
    function resetModal() {
        modal.classList.add('hidden');
        modal.classList.remove('flex');
        transactionForm.reset();
        transactionCategorySelect.value = ""; // Reset category selection
        transactionAccountSelect.value = ""; // Reset account selection
        isModalDirty = false; // Reset the dirty flag
    }

    // Update the type button selection
    function updateTypeSelection(type) {
        // Set button styles based on selected type
        if (type === 'Expense') {
            createExpenseButton.classList.add('bg-blue-500', 'text-white');
            createExpenseButton.classList.remove('bg-gray-200', 'text-gray-800');
            createIncomeButton.classList.remove('bg-blue-500', 'text-white');
            createIncomeButton.classList.add('bg-gray-200', 'text-gray-800');
        } else {
            createIncomeButton.classList.add('bg-blue-500', 'text-white');
            createIncomeButton.classList.remove('bg-gray-200', 'text-gray-800');
            createExpenseButton.classList.remove('bg-blue-500', 'text-white');
            createExpenseButton.classList.add('bg-gray-200', 'text-gray-800');
        }
    }

    // Filter categories based on transaction type
    function filterCategories(type) {
        const options = transactionCategorySelect.querySelectorAll('option');
        let hasCategories = false; // Track if any category is available

        options.forEach(option => {
            if (option.dataset.type === type || option.value === "") {
                option.style.display = 'block'; // Show relevant options
                if (option.value !== "") {
                    hasCategories = true; // At least one category is available
                }
            } else {
                option.style.display = 'none'; // Hide irrelevant options
            }
        });

        transactionCategorySelect.value = ""; // Reset selection

        // Check if no categories are available for the selected type
        if (!hasCategories) {
            alert(`No ${type.toLowerCase()} categories available.`);
        }
    }

    // Filter accounts based on transaction type
    function filterAccounts(type) {
        const options = transactionAccountSelect.querySelectorAll('option');
        let hasAccounts = false; // Track if any account is available

        options.forEach(option => {
            option.style.display = 'block'; // Show all options for now
            if (option.value !== "") {
                hasAccounts = true; // At least one account is available
            }
        });

        transactionAccountSelect.value = ""; // Reset selection

        // Check if no accounts are available
        if (!hasAccounts) {
            alert(`No ${type.toLowerCase()} accounts available.`);
        }
    }

    // Event listeners
    closeModalButton.addEventListener('click', closeAddTransactionModal);
    addTransactionButton.addEventListener('click', openAddTransactionModal);

    // Handle type selection
    createExpenseButton.addEventListener('click', () => {
        transactionType = 'Expense'; // Update transaction type
        updateTypeSelection(transactionType); // Update button styles
        filterCategories(transactionType); // Filter categories based on selection
        filterAccounts(transactionType); // Filter accounts based on selection
    });

    createIncomeButton.addEventListener('click', () => {
        transactionType = 'Income'; // Update transaction type
        updateTypeSelection(transactionType); // Update button styles
        filterCategories(transactionType); // Filter categories based on selection
        filterAccounts(transactionType); // Filter accounts based on selection
    });

    // Track changes in the form (mark as dirty)
    transactionForm.addEventListener('input', () => {
        isModalDirty = true; // Set dirty flag when form input is changed
    });

    // Handle form submission
    // Handle form submission
    transactionForm.addEventListener('submit', function (event) {
        event.preventDefault(); // Prevent default form submission
        const formData = new FormData(this);

        // Log form data for debugging
        for (var pair of formData.entries()) {
            console.log(pair[0] + ': ' + pair[1]); // Check if the data is correct
        }

        fetch('/Transaction/Create', {
            method: 'POST',
            body: formData
        })
            .then(response => {
                if (!response.ok) {
                    return response.json().then(text => {
                        console.error('Error response:', text);
                        throw new Error('Network response was not ok');
                    });
                }
                return response.json();
            })
            .then(response => {
                if (response.success) {
                    // Success handling
                    alert(response.message); // Optionally show a success message
                    location.reload(); // Refresh the page after a successful POST
                } else {
                    // Handle validation errors from the server
                    if (response.errors) {
                        console.error('Validation errors:', response.errors); // Log validation errors for debugging
                        let errorMessages = '';
                        for (let field in response.errors) {
                            errorMessages += `${field}: ${response.errors[field].join(', ')}\n`;
                        }
                        alert(`Validation errors:\n${errorMessages}`); // Display error messages
                    } else {
                        console.error('Unexpected error format:', response);
                        alert(response.message || 'Validation error occurred.');
                    }
                }
            })
            .catch(error => {
                console.error('There was a problem with the fetch operation:', error);
                alert('An error occurred. Please try again later.');
            });
    });

});
