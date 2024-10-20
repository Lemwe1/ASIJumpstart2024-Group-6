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
    const transactionAccountSelect = document.getElementById('transactionAccount');
    const transactionType = document.getElementById('transactionType');

    let isModalDirty = false;
    

    // Open modal for adding a new transaction
    function openAddTransactionModal() {
        modal.classList.remove('hidden');
        modal.classList.add('flex');
        resetTransactionForm();
        updateTypeSelection('Expense'); // Set default type to 'Expense'
        filterCategories('Expense'); // Filter categories based on type
    }

    // Open modal for editing a transaction
    function openEditTransactionModal(transaction) {
    editModal.classList.remove('hidden');
    editModal.classList.add('flex');
    populateEditModalFields(transaction);
    updateTypeSelection(transaction.transactionType);
    filterCategories(transaction.transactionType);
    filterAccounts('debit');
}



    // Reset form fields when the reset button is clicked
    document.getElementById('resetTransactionForm').addEventListener('click', function () {
        // Reset the form fields
        document.getElementById('transactionForm').reset();

        // Reset category and account selections
        const transactionCategorySelect = document.getElementById('transactionCategory');
        const transactionAccountSelect = document.getElementById('transactionAccount');

        // Resetting to default (assuming the first option is disabled)
        transactionCategorySelect.value = "";
        transactionAccountSelect.value = "";
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
        editModal.classList.remove('flex'); // Ensure edit modal is also hidden
        resetTransactionForm();
    }

    // Reset the transaction form
    function resetTransactionForm() {
        transactionForm.reset();
        transactionCategorySelect.value = "";
        transactionAccountSelect.value = "";
        isModalDirty = false; // Reset the dirty flag
        currentTransactionId = null; // Reset transaction ID
    }

    // Populate edit modal fields with transaction data
    function populateEditModalFields(transaction) {
        if (!transaction || typeof transaction !== 'object') {
            console.error("Invalid transaction object");
            return;
        }



        const { transactionId, amount, transactionDate, note, categoryId, deLiId, transactionType } = transaction;

        currentTransactionId = transaction.transactionId;

        document.getElementById('editTransactionId').value = transactionId;
        document.getElementById('editTransactionAmount').value = amount ? amount.toFixed(2) : '0.00';
        document.getElementById('editTransactionDate').value = transactionDate ? new Date(transactionDate).toISOString().slice(0, 10) : '';
        document.getElementById('editTransactionNote').value = note || '';
        document.getElementById('editTransactionCategory').value = categoryId || '';
        document.getElementById('editTransactionAccount').value = deLiId || '';
        document.getElementById('editTransactionType').value = transactionType || 'Expense';
    }


    // Function to update button styles based on the type
    function updateButtonStyles(button, isExpense) {
        button.classList.toggle('bg-blue-500', isExpense);
        button.classList.toggle('text-white', isExpense);
        button.classList.toggle('bg-gray-200', !isExpense);
        button.classList.toggle('text-gray-800', !isExpense);
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
        updateButtonStyles(createExpenseButton, isExpense);
        updateButtonStyles(createIncomeButton, !isExpense);

        // Update styles for edit buttons
        updateButtonStyles(editExpenseButton, isExpense);
        updateButtonStyles(editIncomeButton, !isExpense);
    }

    // Type selection buttons for creating transactions
    createExpenseButton.addEventListener('click', () => {
        updateTypeSelection('Expense');
        filterCategories('Expense');
        filterAccounts('debit');
    });

    createIncomeButton.addEventListener('click', () => {
        updateTypeSelection('Income');
        filterCategories('Income');
        filterAccounts('debit');
    });

    // Type selection buttons for editing transactions
    editExpenseButton.addEventListener('click', () => {
        updateTypeSelection('Expense');
        filterCategories('Expense');
        filterAccounts('debit');
    });

    editIncomeButton.addEventListener('click', () => {
        updateTypeSelection('Income');
        filterCategories('Income');
        filterAccounts('debit');
    });


    // Filter categories based on transaction type
    function filterCategories(type) {
        const options = transactionCategorySelect.querySelectorAll('option');
        options.forEach(option => {
            option.style.display = (option.dataset.type === type || option.value === "") ? 'block' : 'none';
        });
        transactionCategorySelect.value = ""; // Reset selection
    }

    // Filter accounts based on DeLiType (e.g., debit or borrowed)
    function filterAccounts(deLiType) {
        const options = transactionAccountSelect.querySelectorAll('option');
        options.forEach(option => {
            // Display only accounts that match the specified deLiType (e.g., 'debit')
            option.style.display = (option.dataset.type === deLiType || option.value === "") ? 'block' : 'none';
        });
        transactionAccountSelect.value = ""; // Reset selection
    }

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
    // Handle editing an existing transaction
    function handleEditTransaction(event) {
        event.preventDefault();

        // Check if the currentTransactionId is set
        if (!currentTransactionId) {
            Swal.fire({
                title: 'Error',
                text: 'Transaction ID is not set. Please select a transaction to edit.',
                icon: 'error',
                customClass: { popup: 'swal2-front' }
            });
            return; // Exit the function if there is no transaction ID
        }

        // Collect form data
        const formData = new FormData(editTransactionForm);
        const data = {}; // Object to hold the JSON data

        // Convert FormData to a plain object
        formData.forEach((value, key) => {
            data[key] = value;
        });

        // Add the TransactionId to the data object
        data.TransactionId = currentTransactionId;

        // Log the data being sent for debugging
        console.log('Data being sent:', JSON.stringify(data));

        // Send the request using JSON
        fetch(`/Transaction/Edit/${currentTransactionId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json' // Set content type to JSON
            },
            body: JSON.stringify(data) // Send the data as a JSON string
        })
            .then(handleResponse)
            .catch(handleError);
    }





    // Response handler for adding/editing transactions
    function handleResponse(response) {
        if (!response.ok) {
            throw new Error('An error occurred while processing your request.');
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
                title: 'Are you sure?',
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