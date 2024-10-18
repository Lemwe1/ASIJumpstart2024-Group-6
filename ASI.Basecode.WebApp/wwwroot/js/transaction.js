document.addEventListener("DOMContentLoaded", () => {
    const modal = document.getElementById('addTransactionModal');
    const addTransactionButton = document.getElementById('openAddTransactionModal');
    const closeModalButton = document.getElementById('closeTransactionModal');
    const transactionForm = document.getElementById('transactionForm');
    const createExpenseButton = document.getElementById('createExpenseButton');
    const createIncomeButton = document.getElementById('createIncomeButton');
    const transactionCategorySelect = document.getElementById('transactionCategory');
    const transactionAccountSelect = document.getElementById('transactionAccount');
    const addDebitType = document.getElementById('addDebitType'); // Assuming element exists
    const addBorrowedType = document.getElementById('addBorrowedType'); // Assuming element exists
    const transactionType = document.getElementById('transactionType');

    let isModalDirty = false;
    let currentTransactionId = null; // Store the current transaction ID for edits

    // Open modal for adding a new transaction
    function openAddTransactionModal() {
        modal.classList.remove('hidden');
        modal.classList.add('flex');
        transactionForm.reset(); // Reset form when opening modal
        currentTransactionId = null; // Ensure ID is null for add
        updateTypeSelection('Expense'); // Set default type to 'Expense'
        filterCategories('Expense'); // Filter categories based on type
        filterAccounts('Expense'); // Filter accounts based on type
        isModalDirty = false; // Reset dirty flag
    }

    // Open modal for editing a transaction
    function openEditTransactionModal(transaction) {
        modal.classList.remove('hidden');
        modal.classList.add('flex');
        transactionForm.reset(); // Reset form before populating
        populateTransactionData(transaction); // Populate form with transaction data
        updateTypeSelection(transaction.Type); // Ensure correct type button is highlighted
        filterCategories(transaction.Type); // Filter categories based on transaction type
        filterAccounts(transaction.Type); // Filter accounts based on transaction type
        isModalDirty = false; // Reset dirty flag
    }

    // Close modal with confirmation if there are unsaved changes
    function closeAddTransactionModal() {
        if (isModalDirty) {
            if (confirm("You have unsaved changes. Do you want to discard them?")) {
                resetModal();
            }
        } else {
            resetModal();
        }
    }

    // Reset modal state
    function resetModal() {
        modal.classList.add('hidden');
        modal.classList.remove('flex');
        transactionForm.reset();
        transactionCategorySelect.value = ""; // Reset category selection
        transactionAccountSelect.value = ""; // Reset account selection
        isModalDirty = false; // Reset the dirty flag
        currentTransactionId = null; // Reset transaction ID
    }

    // Update the type button selection
    function updateTypeSelection(type) {
        if (transactionType) {
            transactionType.value = type;
        } else {
            console.error('Transaction Type element not found.');
            return;
        }

        if (type === 'Expense') {
            createExpenseButton.classList.add('bg-blue-500', 'text-white');
            createIncomeButton.classList.remove('bg-blue-500', 'text-white');
            createIncomeButton.classList.add('bg-gray-200', 'text-gray-800');
        } else {
            createIncomeButton.classList.add('bg-blue-500', 'text-white');
            createExpenseButton.classList.remove('bg-blue-500', 'text-white');
            createExpenseButton.classList.add('bg-gray-200', 'text-gray-800');
        }
    }

    // Type selection buttons
    createExpenseButton.addEventListener('click', () => {
        updateTypeSelection('Expense');
        filterCategories('Expense');
        filterAccounts('Expense');
    });

    createIncomeButton.addEventListener('click', () => {
        updateTypeSelection('Income');
        filterCategories('Income');
        filterAccounts('Income');
    });

    // Filter categories based on transaction type
    function filterCategories(type) {
        const options = transactionCategorySelect.querySelectorAll('option');
        let hasCategories = false;

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

        if (!hasCategories) {
            alert(`No ${type.toLowerCase()} categories available.`);
        }
    }

    // Filter accounts based on transaction type
    function filterAccounts(type) {
        const options = transactionAccountSelect.querySelectorAll('option');
        let hasAccounts = false;

        options.forEach(option => {
            option.style.display = 'block'; // Show all options for now
            if (option.value !== "") {
                hasAccounts = true; // At least one account is available
            }
        });

        transactionAccountSelect.value = ""; // Reset selection

        if (!hasAccounts) {
            alert(`No ${type.toLowerCase()} accounts available.`);
        }
    }

    // Populate modal fields with transaction data
    function populateTransactionData(transaction) {
        transactionForm.querySelector('input[name="TransactionId"]').value = transaction.transactionId; // Assuming Id is the primary key
        transactionForm.querySelector('input[name="Description"]').value = transaction.Description;
        transactionForm.querySelector('input[name="Amount"]').value = transaction.Amount;
        transactionCategorySelect.value = transaction.CategoryId; // Assuming CategoryId exists in transaction
        transactionAccountSelect.value = transaction.AccountId; // Assuming AccountId exists in transaction
    }

    // Handle adding a new transaction
    function handleAddTransaction(event) {
        event.preventDefault();
        const formData = new FormData(transactionForm); // Collect form data

        fetch('/Transaction/Create', { // Adjust URL for adding transaction
            method: 'POST',
            body: formData
        })
            .then(response => {
                // Check if the response is OK (status in the range 200-299)
                if (!response.ok) {
                    return response.clone().text().then(errorDetails => {
                        console.error('Error adding transaction:', response.status, response.statusText);
                        console.error('Raw response:', errorDetails);
                        throw new Error('An error occurred while adding the transaction. Please try again later.');
                    });
                }
                return response.json();
            })
            .then(result => {
                if (result.success) {
                    alert("Transaction added successfully!");
                    resetModal();
                    window.location.reload(); // Reload the page to reflect the new transaction
                } else {
                    alert("An error occurred while adding the transaction.");
                }
            })
            .catch(error => {
                console.error('Error adding transaction:', error);
                alert(error.message);
            });
    }

    // Handle editing an existing transaction
    function handleEditTransaction(event) {
        event.preventDefault();
        const formData = new FormData(transactionForm); 

        fetch(`/Transaction/Edit/${currentTransactionId}`, { 
            method: 'POST',
            body: formData
        })
            .then(response => {
                // Check if the response is OK (status in the range 200-299)
                if (!response.ok) {
                    return response.clone().text().then(errorDetails => {
                        console.error('Error updating transaction:', response.status, response.statusText);
                        console.error('Raw response:', errorDetails);
                        throw new Error('An error occurred while updating the transaction. Please try again later.');
                    });
                }
                return response.json();
            })
            .then(result => {
                if (result.success) {
                    alert("Transaction updated successfully!");
                    resetModal();
                    window.location.reload(); // Reload the page to reflect the updated transaction
                } else {
                    alert("An error occurred while updating the transaction.");
                }
            })
            .catch(error => {
                console.error('Error updating transaction:', error);
                alert(error.message);
            });
    }

    // Event listeners for modal open and close
    addTransactionButton.addEventListener('click', openAddTransactionModal);
    closeModalButton.addEventListener('click', closeAddTransactionModal);

    // Submit form for adding/editing transactions
    transactionForm.addEventListener('submit', (event) => {
        if (currentTransactionId) {
            handleEditTransaction(event); // Edit flow
        } else {
            handleAddTransaction(event); // Add flow
        }
    });

    // Track form changes to set the dirty flag
    transactionForm.addEventListener('input', () => {
        isModalDirty = true;
    });

    // Close modal when clicking outside
    window.addEventListener('click', (event) => {
        if (event.target === modal) {
            closeAddTransactionModal();
        }
    });

    document.querySelectorAll('.editTransaction').forEach(button => {
        button.addEventListener('click', (event) => {
            const transactionId = event.target.dataset.transactionId; // Get transaction ID from data attribute
            const transaction = getTransactionById(transactionId); // Function to retrieve transaction data (implement accordingly)
            openEditTransactionModal(transaction); // Open modal with transaction data
            currentTransactionId = transactionId; // Set the current transaction ID for editing
        });
    });
});
