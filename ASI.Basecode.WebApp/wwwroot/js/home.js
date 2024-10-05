// home.js

document.addEventListener("DOMContentLoaded", () => {
    const modal = document.getElementById('addTransactionModal');
    const addTransactionButton = document.getElementById('openAddTransactionModal');
    const closeModalButton = document.getElementById('closeTransactionModal');
    const transactionForm = document.getElementById('transactionForm');
    const createExpenseButton = document.getElementById('createExpenseButton');
    const createIncomeButton = document.getElementById('createIncomeButton');

    let transactionType = 'Expense'; // Default type

    // Open modal function
    function openAddTransactionModal() {
        modal.classList.remove('hidden');
        modal.classList.add('flex');
        transactionForm.reset(); // Reset form when opening modal
        updateTypeSelection(transactionType); // Ensure the type button reflects the default selection
    }

    // Close modal function
    function closeAddTransactionModal() {
        modal.classList.add('hidden');
        modal.classList.remove('flex');
    }

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

    // Event listeners
    closeModalButton.addEventListener('click', closeAddTransactionModal);
    addTransactionButton.addEventListener('click', openAddTransactionModal);

    // Handle type selection
    createExpenseButton.addEventListener('click', () => {
        transactionType = 'Expense';
        updateTypeSelection(transactionType);
    });

    createIncomeButton.addEventListener('click', () => {
        transactionType = 'Income';
        updateTypeSelection(transactionType);
    });

    // Handle form submission
    transactionForm.addEventListener('submit', function (event) {
        event.preventDefault(); // Prevent the default form submission
        const formData = new FormData(this); // Create FormData object
        // Process the form data as needed here
        closeAddTransactionModal(); // Close modal after processing
    });
});