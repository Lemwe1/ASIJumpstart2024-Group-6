document.addEventListener("DOMContentLoaded", () => {
    const modal = document.getElementById('addTransactionModal');
    const editModal = document.getElementById('editTransactionModal');
    const addTransactionButton = document.getElementById('openAddTransactionModal');
    const closeAddModalButton = document.getElementById('closeTransactionModal');
    const closeEditModalButton = document.getElementById('closeEditTransactionModal');

    const transactionForm = document.getElementById('transactionForm');
    const editTransactionForm = document.getElementById('editTransactionForm');

    const createExpenseButton = document.getElementById('createExpenseButton');
    const createIncomeButton = document.getElementById('createIncomeButton');
    const editExpenseButton = document.getElementById('editExpenseButton');
    const editIncomeButton = document.getElementById('editIncomeButton');

    const transactionCategorySelect = document.getElementById('transactionCategory');
    const editTransactionCategorySelect = document.getElementById('editTransactionCategory');

    const transactionWalletSelect = document.getElementById('transactionWallet');
    const transactionType = document.getElementById('transactionType');

    let isModalDirty = false;

    // Function to filter table based on category
    const filterTableByCategory = () => {
        const categoryFilter = document.querySelector('select[name="filterByCategory"]').value; // Get the selected category
        const rows = Array.from(document.querySelectorAll('.transaction-table tbody tr')); // Get all rows

        // Show or hide rows based on selected category
        rows.forEach(row => {
            const rowCategory = row.dataset.category; // Get the category from the row
            if (categoryFilter === 'All' || rowCategory === categoryFilter) {
                row.style.display = ''; // Show matching row
            } else {
                row.style.display = 'none'; // Hide non-matching row
            }
        });
    };

    // Event listener for category filter
    document.querySelector('select[name="filterByCategory"]').addEventListener('change', filterTableByCategory);


    // Open modal for adding a new transaction
    function openAddTransactionModal() {
        modal.classList.remove('hidden');
        modal.classList.add('flex');
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
        populateEditModalFields(transaction);
        updateTypeSelection(transaction.transactionType);
        filterCategories(transaction.transactionType);
    }


    // Reset form fields when the reset button is clicked
    document.getElementById('resetTransactionFormButton').addEventListener('click', function () {
        // Reset the form fields
        document.getElementById('transactionForm').reset();



        // Resetting to default (assuming the first option is disabled)
        transactionCategorySelect.value = "";
        transactionWalletSelect.value = "";
    });

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
        resetTransactionForm();
    }

    // Reset the transaction form
    function resetTransactionForm() {
        transactionForm.reset();
        transactionCategorySelect.selectedIndex = -1;
        transactionWalletSelect.selectedIndex = -1;
        isModalDirty = false; // Reset the dirty flag
        currentTransactionId = null; // Reset transaction ID
    }

    // Populate edit modal fields with transaction data
    function populateEditModalFields(transaction) {
        if (!transaction || typeof transaction !== 'object') {
            console.error("Invalid transaction object");
            return;
        }



        const { transactionId, amount, transactionDate, note, categoryId, walletId, transactionType } = transaction;

        currentTransactionId = transaction.transactionId;

        document.getElementById('editTransactionId').value = transactionId;
        document.getElementById('editTransactionAmount').value = amount ? amount.toFixed(2) : '0.00';
        document.getElementById('editTransactionDate').value = transactionDate ? new Date(transactionDate).toISOString().slice(0, 10) : '';
        document.getElementById('editTransactionNote').value = note || '';
        document.getElementById('editTransactionCategory').value = categoryId || '';
        document.getElementById('editTransactionWallet').value = walletId || '';
        document.getElementById('editTransactionType').value = transactionType || 'Expense';


        console.log("Transaction Data:", transaction);
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



    // Function to handle input limiting
    const handleAmountInput = (inputElement) => {
        inputElement.addEventListener('input', (event) => {
            // Get the current value without formatting
            let currentValue = inputElement.value;

            // Remove all non-digit characters and decimal point
            currentValue = currentValue.replace(/[^0-9.]/g, '');

            // Allow only one decimal point
            if (currentValue.match(/\./g).length > 1) {
                currentValue = currentValue.substring(0, currentValue.lastIndexOf('.'));
            }

            // Check if the input value is greater than one trillion
            if (currentValue && Number(currentValue) > 1e12) {
                // Show SweetAlert error message
                Swal.fire({
                    title: 'Error',
                    text: 'Amount must be less than or equal to one trillion.',
                    icon: 'error'
                });

                // Clear the input field
                inputElement.value = ''; // Clear the input to prevent submission errors
                return; // Exit the function to prevent further processing
            }

            // Set the cleaned value back to the input
            inputElement.value = currentValue; // Update input with cleaned value
        });
    };

    const transactionAmountInput = document.getElementById('transactionAmount');
    const editTransactionAmountInput = document.getElementById('editTransactionAmount');

    handleAmountInput(transactionAmountInput);
    handleAmountInput(editTransactionAmountInput);

    // Handle adding a new transaction
    function handleAddTransaction(event) {
        event.preventDefault();
        const formData = new FormData(transactionForm);

        // Convert FormData to a plain object (if needed)
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

    // Handle network errors
    function handleError(error) {
        console.error('Network error:', error);
        Swal.fire({
            title: 'Error',
            text: 'A network error occurred. Please try again later.',
            icon: 'error',
            customClass: { popup: 'swal2-front' }
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
        isModalDirty = true; // Mark the modal as dirty when user interacts with the form
    });

    editTransactionForm.addEventListener('input', () => {
        isModalDirty = true; // Mark the edit modal as dirty when user interacts with the form
    });

    // Attach event listeners for modal opening
    addTransactionButton.addEventListener('click', openAddTransactionModal);
    closeAddModalButton.addEventListener('click', closeTransactionModal);
    closeEditModalButton.addEventListener('click', closeEditTransactionModal);

    // Handle form submission
    transactionForm.addEventListener('submit', handleAddTransaction);
    editTransactionForm.addEventListener('submit', handleEditTransaction);
});