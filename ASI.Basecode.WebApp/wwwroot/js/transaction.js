document.addEventListener("DOMContentLoaded", () => {
    const modal = document.getElementById('addTransactionModal');
    const addTransactionButton = document.getElementById('openAddTransactionModal');
    const closeModalButton = document.getElementById('closeTransactionModal');
    const transactionForm = document.getElementById('transactionForm');
    const createExpenseButton = document.getElementById('createExpenseButton');
    const createIncomeButton = document.getElementById('createIncomeButton');
    const transactionCategorySelect = document.getElementById('transactionCategory');
    const transactionAccountSelect = document.getElementById('transactionAccount'); // New account select

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
                modal.classList.add('hidden');
                modal.classList.remove('flex');
            }
        } else {
            modal.classList.add('hidden');
            modal.classList.remove('flex');
        }
    }

    // Handle outside click to close modal
    window.addEventListener('click', (event) => {
        if (event.target === modal) {
            closeAddTransactionModal();
        }
    });

    // Update the type button selection
    function updateTypeSelection(type) {
        if (type === 'Expense') {
            createExpenseButton.classList.add('bg-blue-500', 'text-white');
            createExpenseButton.classList.remove('bg-white', 'text-gray-700');
            createIncomeButton.classList.remove('bg-blue-500', 'text-white');
            createIncomeButton.classList.add('bg-gray-200', 'text-gray-800');
        } else {
            createIncomeButton.classList.add('bg-blue-500', 'text-white');
            createIncomeButton.classList.remove('bg-white', 'text-gray-700');
            createExpenseButton.classList.remove('bg-blue-500', 'text-white');
            createExpenseButton.classList.add('bg-gray-200', 'text-gray-800');
        }
    }

    // Filter categories based on transaction type
    function filterCategories(type) {
        const options = transactionCategorySelect.querySelectorAll('option');
        options.forEach(option => {
            // Show options matching the transaction type or the default "Select category"
            option.style.display = (option.dataset.type === type || option.value === "") ? 'block' : 'none';
        });
        transactionCategorySelect.value = ""; // Reset selection
    }

    // Filter accounts based on transaction type
    function filterAccounts(type) {
        const options = transactionAccountSelect.querySelectorAll('option');
        options.forEach(option => {
            option.style.display = 'block'; // Show all options
        });
        transactionAccountSelect.value = ""; // Reset selection
    }

    // Event listeners
    closeModalButton.addEventListener('click', closeAddTransactionModal);
    addTransactionButton.addEventListener('click', openAddTransactionModal);

    // Handle type selection
    createExpenseButton.addEventListener('click', () => {
        transactionType = 'Expense';
        updateTypeSelection(transactionType);
        filterCategories(transactionType); // Filter categories based on selection
        filterAccounts(transactionType); // Filter accounts based on selection
    });

    createIncomeButton.addEventListener('click', () => {
        transactionType = 'Income';
        updateTypeSelection(transactionType);
        filterCategories(transactionType); // Filter categories based on selection
        filterAccounts(transactionType); // Filter accounts based on selection
    });

    // Track changes in the form (mark as dirty)
    transactionForm.addEventListener('input', () => {
        isModalDirty = true; // Set dirty flag when form input is changed
    });

    // Handle form submission
    transactionForm.addEventListener('submit', function (event) {
        event.preventDefault(); // Prevent the default form submission
        const formData = new FormData(this); // Create FormData object
        // Process the form data as needed here
        closeAddTransactionModal(); // Close modal after processing
    });
});
