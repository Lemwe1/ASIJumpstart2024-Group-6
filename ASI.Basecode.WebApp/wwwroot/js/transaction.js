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
    let currentTransactionId = null; // Store the current transaction ID for edits

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
        populateEditModalFields(transaction); // Populate fields with transaction data
    }

    // Reset form fields when the reset button is clicked
    document.getElementById('resetTransactionForm').addEventListener('click', function () {
        // Reset the form fields
        document.getElementById('transactionForm').reset();

        // Reset category and account selections
        const transactionCategorySelect = document.getElementById('transactionCategory');
        const transactionAccountSelect = document.getElementById('transactionAccount');

        // Resetting to default (assuming the first option is disabled)
        transactionCategorySelect.value = ""; // Reset category selection
        transactionAccountSelect.value = ""; // Reset account selection
    });

    // Close modal with confirmation if there are unsaved changes
    function closeTransactionModal() {
        if (isModalDirty) {
            Swal.fire({
                title: 'You have unsaved changes.',
                text: "Do you want to discard them?",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, discard it!',
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
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, discard it!',
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
        document.getElementById('editTransactionAmount').value = transaction.amount;
        document.getElementById('editTransactionDate').value = new Date(transaction.transactionDate).toISOString().slice(0, 10); // Format date if necessary
        document.getElementById('editTransactionNote').value = transaction.note || ''; // Use empty string if null
        document.getElementById('editTransactionCategory').value = transaction.categoryId; // Set the selected category
        document.getElementById('editTransactionAccount').value = transaction.deLiId; // Set the selected account
        document.getElementById('editTransactionType').value = transaction.transactionType || 'Expense'; // Set the transaction type (default to Expense if null)
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
        createExpenseButton.classList.toggle('bg-blue-500', isExpense);
        createExpenseButton.classList.toggle('text-white', isExpense);
        createIncomeButton.classList.toggle('bg-blue-500', !isExpense);
        createIncomeButton.classList.toggle('text-white', !isExpense);
        createIncomeButton.classList.toggle('bg-gray-200', isExpense);
        createIncomeButton.classList.toggle('text-gray-800', isExpense);
    }

    // Type selection buttons
    createExpenseButton.addEventListener('click', () => {
        updateTypeSelection('Expense');
        filterCategories('Expense');
    });

    createIncomeButton.addEventListener('click', () => {
        updateTypeSelection('Income');
        filterCategories('Income');
    });

    // Filter categories based on transaction type
    function filterCategories(type) {
        const options = transactionCategorySelect.querySelectorAll('option');
        options.forEach(option => {
            option.style.display = (option.dataset.type === type || option.value === "") ? 'block' : 'none';
        });
        transactionCategorySelect.value = ""; // Reset selection
    }

    // Handle adding a new transaction
    function handleAddTransaction(event) {
        event.preventDefault();
        const formData = new FormData(transactionForm); // Collect form data
        fetch('/Transaction/Create', { method: 'POST', body: formData })
            .then(handleResponse)
            .catch(handleError);
    }

    // Handle editing an existing transaction
    function handleEditTransaction(event) {
        event.preventDefault();
        const formData = new FormData(transactionForm);
        fetch(`/Transaction/Edit/${currentTransactionId}`, { method: 'POST', body: formData })
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
                            confirmButtonText: 'Okay'
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
                            confirmButtonText: 'Okay'
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

    // Event listeners for closing the modal
    closeAddModalButton.addEventListener('click', closeTransactionModal);
    closeEditModalButton.addEventListener('click', closeEditTransactionModal);

    // Open the modal on button click
    addTransactionButton.addEventListener('click', openAddTransactionModal);

    // Form submission event listener
    transactionForm.addEventListener('submit', handleAddTransaction);
});
