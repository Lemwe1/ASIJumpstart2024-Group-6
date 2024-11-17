document.addEventListener("DOMContentLoaded", () => {
    const modal = document.getElementById('addTransactionModal');

    //add
    const addTransactionButton = document.getElementById('openAddTransactionModal');
    const closeAddModalButton = document.getElementById('closeTransactionModal');
    const transactionForm = document.getElementById('transactionForm');

    const createExpenseButton = document.getElementById('createExpenseButton');
    const createIncomeButton = document.getElementById('createIncomeButton');
    const transactionCategorySelect = document.getElementById('transactionCategory');
    const transactionAmountInput = document.getElementById('transactionAmount');
    const transactionWalletSelect = document.getElementById('transactionWallet');
    const transactionType = document.getElementById('transactionType');
    const walletBalanceSpan = document.getElementById('walletBalance');

    //edit
    const editModal = document.getElementById('editTransactionModal');
    const closeEditModalButton = document.getElementById('closeEditTransactionModal');
    const editTransactionForm = document.getElementById('editTransactionForm');
    const editExpenseButton = document.getElementById('editExpenseButton');
    const editIncomeButton = document.getElementById('editIncomeButton');
    const editTransactionCategorySelect = document.getElementById('editTransactionCategory');
    const editTransactionAmountInput = document.getElementById('editTransactionAmount');
    const editTransactionWalletSelect = document.getElementById('editTransactionWallet');
    const editTransactionType = document.getElementById('editTransactionType');
    const editWalletBalanceSpan = document.getElementById('editWalletBalance');



    let isModalDirty = false;
    // Function to filter table based on category
    const filterTableByCategory = () => {
        const categoryFilter = document.querySelector('select[name="filterByCategory"]').value; // Get the selected category
        const rows = Array.from(document.querySelectorAll('.transaction-table tbody tr[data-category]')); // Get all rows with category data
        let hasVisibleTransactions = false;

        // Show or hide rows based on selected category
        rows.forEach(row => {
            const rowCategory = row.dataset.category; // Get the category from the row
            if (categoryFilter === 'All' || rowCategory === categoryFilter) {
                row.style.display = ''; // Show matching row
                hasVisibleTransactions = true;
            } else {
                row.style.display = 'none'; // Hide non-matching row
            }
        });

        // Check if "No transactions found" row already exists
        let noTransactionsRow = document.querySelector('.transaction-table tbody .no-transactions-row');
        if (!noTransactionsRow) {
            // Create "No transactions found" row if it doesn't exist
            noTransactionsRow = document.createElement('tr');
            noTransactionsRow.classList.add('no-transactions-row');
            noTransactionsRow.innerHTML = `<td colspan="6" class="px-4 py-3 text-center text-gray-500 dark:text-white">&#x1F937; No transactions found.</td>`;
            document.querySelector('.transaction-table tbody').appendChild(noTransactionsRow);
        }
        // Show "No transactions found" message only if no rows are visible
        noTransactionsRow.style.display = hasVisibleTransactions ? 'none' : '';
    };

    // Initial filter on page load
    filterTableByCategory();

    // Event listener for category filter
    document.querySelector('select[name="filterByCategory"]').addEventListener('change', filterTableByCategory);

    // Open modal for adding a new transaction, with wallet check
    function openAddTransactionModal() {
        // Check if there are no wallets (only the placeholder option is present)
        if (transactionWalletSelect.options.length <= 1) {
            Swal.fire({
                icon: 'warning',
                title: 'No Wallets Found',
                text: 'It looks like there are no wallets available. Please add a wallet first!',
                confirmButtonColor: '#3B82F6',
                confirmButtonText: 'Add Wallet',
                showCancelButton: true,
                cancelButtonText: 'Cancel'
            }).then((result) => {
                if (result.isConfirmed) {
                    window.location.href = '/Wallet/Index'; // Redirect to add wallet page
                }
            });
            return; // Stop the function from proceeding to open the modal
        }

        // If wallets are available, proceed to open the modal
        modal.classList.remove('hidden');
        modal.classList.add('flex');
        document.body.classList.add('overflow-hidden');
        resetTransactionForm();
        updateTypeSelection('Expense');
        filterCategories('Expense');
        transactionCategorySelect.value = "";
        transactionWalletSelect.value = "";
    }


    // Open modal for editing a transaction
    function openEditTransactionModal(transaction) {
        editModal.classList.remove('hidden');
        editModal.classList.add('flex');
        document.body.classList.add('overflow-hidden');
        editErrorMessage.style.display = 'none';
        populateEditModalFields(transaction);
        updateTypeSelection(transaction.transactionType);
        filterCategories(transaction.transactionType);
        updateWalletBalance(editTransactionWalletSelect, editWalletBalanceSpan);
    }

    // Close modal with confirmation if there are unsaved changes
    function closeTransactionModal() {
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
                if (result.isConfirmed) resetModal();
            });
        } else {
            resetModal();
        }
    }


    // Close the edit modal with a confirmation prompt
    function closeEditTransactionModal() {
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
                resetModal();
            }
        });
    }
    // Add event listener to the cancel button to close the edit modal
    document.getElementById('cancelButton').addEventListener('click', function () {
        closeEditTransactionModal();
    });

    // Close the modal when clicking outside of it
    window.addEventListener('click', (event) => {
        if (event.target === modal) {
            closeTransactionModal(); // Close the add transaction modal
        } else if (event.target === editModal) {
            closeEditTransactionModal(); // Close the edit transaction modal
        }
    });

    // Reset modal state
    function resetModal() {
        modal.classList.add('hidden');
        modal.classList.remove('flex');
        editModal.classList.add('hidden');
        editModal.classList.remove('flex');
        document.body.classList.remove('overflow-hidden');
        resetTransactionForm();
    }

    // Reset the transaction form
    function resetTransactionForm() {
        transactionForm.reset(); // Reset the form fields
        transactionCategorySelect.selectedIndex = -1;
        transactionWalletSelect.selectedIndex = -1;
        walletBalanceSpan.textContent = '';
        walletBalanceSpan.style.display = 'none';
        errorMessage.textContent = ''; // Clear error message
        isModalDirty = false; // Reset the dirty flag
        currentTransactionId = null; // Reset transaction ID
    }

    // Populate edit modal fields with transaction data
    function populateEditModalFields(transaction) {
        if (!transaction || typeof transaction !== 'object') {
            console.error("Invalid transaction object");
            return;
        }

        const { transactionId, amount, transactionDate, note, categoryId, walletId, transactionType, transactionSort } = transaction;

        currentTransactionId = transaction.transactionId;

        document.getElementById('editTransactionId').value = transactionId;
        document.getElementById('editTransactionAmount').value = amount ? amount.toFixed(2) : '0.00';

        if (transactionDate) {
            // Convert the date to local date string
            const localDate = new Date(transactionDate);
            const localDateString = localDate.toLocaleDateString('en-CA'); // 'en-CA' gives the YYYY-MM-DD format
            document.getElementById('editTransactionDate').value = localDateString;
        } else {
            document.getElementById('editTransactionDate').value = '';
        }

        document.getElementById('editTransactionNote').value = note || '';
        document.getElementById('editTransactionCategory').value = categoryId || '';
        document.getElementById('editTransactionWallet').value = walletId || '';
        document.getElementById('editTransactionType').value = transactionType || 'Expense';
        document.getElementById('editTransactionSort').value = transactionSort;

        console.log("Transaction Data:", transaction);
    }

    // Type selection buttons for creating transactions
    createExpenseButton.addEventListener('click', () => handleTypeSelection('Expense'));
    createIncomeButton.addEventListener('click', () => handleTypeSelection('Income'));

    // Type selection buttons for editing transactions
    editExpenseButton.addEventListener('click', () => handleTypeSelection('Expense'));
    editIncomeButton.addEventListener('click', () => handleTypeSelection('Income'));



    function filterCategories(type) {
        // Get all category options from both modals
        const transactionCategoryOptions = transactionCategorySelect.querySelectorAll('option');
        const editTransactionCategoryOptions = editTransactionCategorySelect.querySelectorAll('option');

        // Filter options in the add modal
        transactionCategoryOptions.forEach(option => {
            option.style.display = (option.dataset.type === type || option.value === "") ? 'block' : 'none';
        });
        transactionCategorySelect.value = ""; // Reset selection in add modal

        // Filter options in the edit modal
        editTransactionCategoryOptions.forEach(option => {
            option.style.display = (option.dataset.type === type || option.value === "") ? 'block' : 'none';
        });
    }
    // Function to update the wallet balance display
    function updateWalletBalance(selectElement, balanceSpan) {
        const selectedOption = selectElement.options[selectElement.selectedIndex];
        const balance = selectedOption ? parseFloat(selectedOption.getAttribute('data-balance')) : null;

        if (balance !== null) {
            // Format the balance with commas and two decimal places
            const formattedBalance = balance.toLocaleString('en-US', {
                style: 'currency',
                currency: 'PHP',
                minimumFractionDigits: 2,
                maximumFractionDigits: 2
            });

            balanceSpan.textContent = `(Available Balance: ${formattedBalance})`;
            balanceSpan.style.display = 'block'; // Show the balance span
        } else {
            balanceSpan.style.display = 'none'; // Hide the balance span if no wallet is selected
        }
    }


    // Event listener for the transaction wallet selection (Create Transaction)
    transactionWalletSelect.addEventListener('change', function () {
        updateWalletBalance(transactionWalletSelect, walletBalanceSpan);
    });

    // Event listener for the edit transaction wallet selection (Edit Transaction)
    editTransactionWalletSelect.addEventListener('change', function () {
        updateWalletBalance(editTransactionWalletSelect, editWalletBalanceSpan);
    });

    // Create an error message element for validation (for both transaction and edit elements)
    function createErrorMessage(inputElement) {
        const errorMessage = document.createElement('div');
        errorMessage.style.color = 'red';
        errorMessage.style.fontSize = '0.875em';
        errorMessage.style.marginTop = '0.25em';
        errorMessage.style.display = 'none'; // Initially hidden
        inputElement.parentNode.insertBefore(errorMessage, inputElement.nextSibling);
        return errorMessage;
    }

    // For both transaction and edit input elements, create error messages
    const errorMessage = createErrorMessage(transactionAmountInput);
    const editErrorMessage = createErrorMessage(editTransactionAmountInput);

    // Live validation for transaction and edit input fields
    function validateAmountInput(inputElement, walletSelect, errorMessage) {
        inputElement.addEventListener('input', function () {
            const selectedOption = walletSelect.options[walletSelect.selectedIndex];
            const balance = selectedOption ? parseFloat(selectedOption.getAttribute('data-balance') || 0) : 0;
            const amount = parseFloat(inputElement.value || 0);
            const transactionTypeValue = transactionType.value;

            if (!balance) {
                errorMessage.style.display = 'none';
                return;
            }

            if (transactionTypeValue === "Expense" && amount > balance) {
                inputElement.style.borderColor = 'red';
                errorMessage.textContent = `You do not have enough balance.`;
                errorMessage.style.display = 'block';
            } else {
                inputElement.style.borderColor = '';
                errorMessage.style.display = 'none';
            }
        });
    }


    validateAmountInput(transactionAmountInput, transactionWalletSelect, errorMessage);
    validateAmountInput(editTransactionAmountInput, editTransactionWalletSelect, editErrorMessage);

    // Validation on form submission
    function validateTransaction(event, amountInput, walletSelect, errorMessage, transactionType) {
        event.preventDefault(); // Prevent form submission

        const selectedOption = walletSelect.options[walletSelect.selectedIndex];
        const balance = selectedOption ? parseFloat(selectedOption.getAttribute('data-balance') || 0) : null;
        const amount = parseFloat(amountInput.value || 0);

        if (!balance) {
            toastr.error('Please select a wallet before entering an amount.', 'Validation Error');
            return; // Stop form submission
        }

        if (transactionType === "Expense" && amount > balance) {
            amountInput.style.borderColor = 'red'; // Highlight the input field
            errorMessage.textContent = 'Transaction amount exceeds the current wallet balance for expenses.';
            errorMessage.style.display = 'block'; // Show inline error message

            // Display a Swal warning
            Swal.fire({
                title: 'Validation Error',
                text: 'Transaction amount exceeds the current wallet balance for expenses.',
                icon: 'warning',
                confirmButtonText: 'OK',
                confirmButtonColor: '#3B82F6',
            });

            return; // Stop form submission
        }

        errorMessage.style.display = 'none'; // Hide error message

        // Proceed with form submission (Add or Edit)
        if (amountInput === transactionAmountInput) {
            handleAddTransaction(event);
        } else {
            handleEditTransaction(event);
        }
    }

    // Function to update button styles based on the type
    function updateButtonStyles(button, isSelected, isExpense) {
        // Remove all color classes to avoid overlap
        button.classList.remove('bg-red-400', 'bg-green-500', 'text-white', 'bg-gray-200', 'text-gray-800');

        if (isSelected) {
            // Apply styles based on whether the button is for expense or income
            if (isExpense) {
                button.classList.add('bg-red-400', 'text-white'); // Red for selected expense
            } else {
                button.classList.add('bg-green-500', 'text-white'); // Green for selected income
                errorMessage.style.display = 'none'; // Hide error message

            }
        } else {
            button.classList.add('bg-gray-200', 'text-gray-800'); // Gray for not selected
        }
    }

    // Update the type button selection
    function updateTypeSelection(type) {

        if (transactionType) {
            transactionType.value = type;
        } else {
            console.error('Transaction Type element not found.');
            return;
        }

        const isExpense = type === 'Expense';

        // Update styles for create buttons
        updateButtonStyles(createExpenseButton, isExpense, true);
        updateButtonStyles(createIncomeButton, !isExpense, false);

        // Update styles for edit buttons
        updateButtonStyles(editExpenseButton, isExpense, true);
        updateButtonStyles(editIncomeButton, !isExpense, false);
    }

    // Function to set type and apply filters
    function handleTypeSelection(type) {
        updateTypeSelection(type);
        filterCategories(type);
    }


    // Reset form fields when the reset button is clicked
    document.getElementById('resetTransactionFormButton').addEventListener('click', function () {
        // Reset the form fields
        document.getElementById('transactionForm').reset();

        walletBalanceSpan.textContent = '';
        walletBalanceSpan.style.display = 'none';
        errorMessage.textContent = ''; // Clear error message

        // Resetting to default (assuming the first option is disabled)
        transactionCategorySelect.value = "";
        transactionWalletSelect.value = "";
    });

    // Function to handle adding a new transaction
    function handleAddTransaction(event) {
        const formData = new FormData(transactionForm);

        const data = {};
        formData.forEach((value, key) => {
            data[key] = value;
        });

        // Send the request with JSON data
        fetch('/Transaction/Create', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        })
            .then(handleResponse)
            .catch(handleError);
    }


    function handleEditTransaction(event) {
        event.preventDefault();

        const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
        const token = tokenElement ? tokenElement.value : null;

        if (!currentTransactionId) {
            Swal.fire({
                title: 'Error',
                text: 'Transaction ID is not set. Please select a transaction to edit.',
                icon: 'error'
            });
            return;
        }

        const formData = new FormData(editTransactionForm);
        const data = {};

        formData.forEach((value, key) => {
            data[key] = value;
        });

        // Validate required fields
        if (!data.CategoryId || !data.walletId || !transactionType.value || !data.Amount || !data.TransactionDate) {
            let errorMessage = 'Please fill in all required fields:';
            if (!data.CategoryId) errorMessage += '\n- Category is required.';
            if (!data.walletId) errorMessage += '\n- Wallet is required.';
            if (!transactionType.value) errorMessage += '\n- Transaction Type is required.';
            if (!data.Amount) errorMessage += '\n- Amount is required.';
            if (!data.TransactionDate) errorMessage += '\n- Transaction Date is required.';

            Swal.fire({
                title: 'Error',
                text: errorMessage,
                icon: 'error'
            });
            return;
        }

        // Convert types and format date
        data.CategoryId = parseInt(data.CategoryId, 10);
        data.walletId = parseInt(data.walletId, 10);
        data.TransactionId = currentTransactionId;
        data.TransactionType = transactionType.value;
        data.TransactionDate = new Date(data.TransactionDate).toISOString();
        data.Amount = parseFloat(data.Amount);

        console.log("Data to be sent:", JSON.stringify(data)); // Debugging line

        fetch(`/Transaction/Edit/${currentTransactionId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify(data)
        })
            .then(handleResponse)
            .catch(error => {
                Swal.fire({
                    title: 'Error',
                    text: 'An error occurred: ' + error.message,
                    icon: 'error'
                });
            });
    }

    // Response handler for adding/editing transactions
    function handleResponse(response) {
        if (!response.ok) {
            if (response.status === 400) {
                return response.json().then(errorData => {
                    if (errorData.errors) {
                        let errorMessage = "Please correct the following errors:\n";
                        for (const field in errorData.errors) {
                            errorMessage += `- ${field}: ${errorData.errors[field].join(', ')}\n`;
                        }
                        Swal.fire({
                            title: 'Error',
                            text: errorMessage,
                            icon: 'error',
                            customClass: { popup: 'swal2-front' }
                        });
                    } else {
                        Swal.fire({
                            title: 'Error',
                            text: errorData.message || 'An error occurred.',
                            icon: 'error',
                            customClass: { popup: 'swal2-front' }
                        });
                    }
                });
            } else if (response.status === 401) {
                throw new Error('Unauthorized access.');
            } else if (response.status >= 500) {
                throw new Error('Server error occurred.');
            } else {
                throw new Error('An error occurred while processing your request.');
            }
        }
        return response.json().then(result => {
            if (result.success) {
                Swal.fire({
                    title: 'Success',
                    text: result.message || 'Transaction processed successfully!',
                    icon: 'success',
                    confirmButtonColor: '#3B82F6',
                    customClass: { popup: 'swal2-front' }
                }).then(() => {
                    resetModal();
                    window.location.reload();
                });
            } else {
                Swal.fire({
                    title: 'Error',
                    text: result.message || 'An error occurred.',
                    icon: 'error',
                    customClass: { popup: 'swal2-front' }
                });
            }
        }).catch(error => {
            console.error('Error handling response:', error);
            Swal.fire({
                title: 'Error',
                text: error.message || 'An error occurred. Please try again later.',
                icon: 'error',
                customClass: { popup: 'swal2-front' }
            });
        });
    }

    // Error handler
    function handleError(error) {
        console.error('Transaction processing error:', error);
        Swal.fire({
            title: 'Error',
            text: error.message || 'An unexpected error occurred.',
            icon: 'error',
            customClass: { popup: 'swal2-front' }
        });
    }

    // Edit transaction buttons
    document.querySelectorAll('.editTransaction').forEach(button => {
        button.addEventListener('click', () => {
            const transactionId = button.getAttribute('data-id');
            fetchTransaction(transactionId);
        });
    });

    // Function to fetch and open the transaction for editing
    async function fetchTransaction(transactionId) {
        try {
            const response = await fetch(`/Transaction/GetTransaction/${transactionId}`);
            const result = await response.json();

            if (result.success) {
                const transaction = result.data;
                openEditTransactionModal(transaction); // Open edit modal with transaction data
            } else {
                console.error('Error fetching transaction:', result.message);
                alert(result.message);
            }
        } catch (error) {
            console.error('Error fetching transaction:', error);
            alert('Error fetching transaction data.');
        }
    }

    // Function to handle deletion of a transaction
    document.querySelectorAll('.deleteTransaction').forEach(button => {
        button.addEventListener('click', async () => {
            const id = button.getAttribute('data-id'); // Get the transaction ID from data-id attribute

            const { isConfirmed } = await Swal.fire({
                title: 'Delete Transaction',
                text: 'Do you really want to delete this transaction?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#a6a6a6',
                confirmButtonText: 'Yes, delete it!',
                cancelButtonText: 'No, cancel!'
            });

            if (isConfirmed) {
                const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
                const token = tokenElement ? tokenElement.value : null;

                try {
                    const response = await fetch(`/Transaction/Delete/${id}`, {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                            'RequestVerificationToken': token // CSRF token
                        }
                    });

                    const responseText = await response.text(); // Get response as text

                    if (!response.ok) {
                        console.error('Server responded with an error:', response.status, responseText);
                        await Swal.fire({
                            title: 'Error!',
                            text: 'Failed to delete transaction. Server responded with status ' + response.status,
                            icon: 'error',
                            confirmButtonText: 'Okay',
                            confirmButtonColor: '#3B82F6'
                        });
                        return;
                    }

                    const result = JSON.parse(responseText); // Parse the response text as JSON

                    // Check if deletion was successful
                    if (result.success || result.message === 'Transaction deleted successfully.') {
                        await Swal.fire({
                            title: 'Deleted!',
                            text: 'Your transaction has been deleted.',
                            icon: 'success',
                            confirmButtonText: 'Okay',
                            confirmButtonColor: '#3B82F6'

                        });
                        location.reload(); // Refresh the page
                    } else {
                        console.error('Deletion failed:', result.message);
                        await Swal.fire({
                            title: 'Error!',
                            text: 'Deletion failed: ' + result.message,
                            icon: 'error',
                            confirmButtonText: 'Okay'
                        });
                    }
                } catch (error) {
                    console.error('Error deleting transaction:', error);
                    await Swal.fire({
                        title: 'Error!',
                        text: 'An error occurred while deleting the transaction.',
                        icon: 'error',
                        confirmButtonText: 'Okay'
                    });
                }
            }
        });
    });

    // Mark form as dirty when there are changes
    transactionForm.addEventListener('input', () => {
        isModalDirty = true;
    });

    editTransactionForm.addEventListener('input', () => {
        isModalDirty = true;
    });

    // Attach event listeners for modal opening
    addTransactionButton.addEventListener('click', openAddTransactionModal);
    closeAddModalButton.addEventListener('click', closeTransactionModal);
    closeEditModalButton.addEventListener('click', closeEditTransactionModal);

    // Handle form submission for both transaction and edit forms
    transactionForm.addEventListener('submit', function (event) {
        validateTransaction(event, transactionAmountInput, transactionWalletSelect, errorMessage, transactionType.value);
    });

    document.getElementById('editExpenseButton').addEventListener('click', function () {
        // Set the hidden input value to "Expense" for Edit form
        document.getElementById('editTransactionType').value = 'Expense';
    });

    document.getElementById('editIncomeButton').addEventListener('click', function () {
        // Set the hidden input value to "Income" for Edit form
        document.getElementById('editTransactionType').value = 'Income';
    });

    editTransactionForm.addEventListener('submit', function (event) {
        event.preventDefault(); // Prevent form submission

        // Retrieve the current value of the transaction type from the hidden input field
        const transactionTypeValue = document.getElementById('editTransactionType').value;

        // Now validate the form with the current transaction type value
        validateTransaction(event, editTransactionAmountInput, editTransactionWalletSelect, editErrorMessage, transactionTypeValue);

        console.log('Selected transaction type (Edit):', transactionTypeValue); 
    });
});