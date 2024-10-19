document.addEventListener("DOMContentLoaded", () => {
    const modal = document.getElementById('addTransactionModal');

    const addTransactionButton = document.getElementById('openAddTransactionModal');
    const closeModalButton = document.getElementById('closeTransactionModal');
    const deleteTransactionButton = document.querySelectorAll('.deleteTransaction');

    const transactionForm = document.getElementById('transactionForm');

    const createExpenseButton = document.getElementById('createExpenseButton');
    const createIncomeButton = document.getElementById('createIncomeButton');

    const transactionCategorySelect = document.getElementById('transactionCategory');
    const transactionAccountSelect = document.getElementById('transactionAccount');
    const transactionType = document.getElementById('transactionType');


    let isModalDirty = false;
    let currentTransactionId = null; // Store the current transaction ID for edits

    // Open modal for adding a new transaction
    function openAddTransactionModal() {
        modal.classList.remove('hidden');
        modal.classList.add('flex');
        transactionForm.reset(); // Reset form when opening modal
        currentTransactionId = null; // Ensure ID is null for add
        transactionCategorySelect.value = "";
        transactionAccountSelect.value = "";
        updateTypeSelection('Expense'); // Set default type to 'Expense'
        filterCategories('Expense'); // Filter categories based on type
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
        isModalDirty = false; // Reset dirty flag
    }

    // Close modal with confirmation if there are unsaved changes
    function closeAddTransactionModal() {
        if (isModalDirty) {
            Swal.fire({
                title: 'You have unsaved changes.',
                text: "Do you want to discard them?",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, discard it!',
                customClass: {
                    popup: 'swal2-front' // Custom class to control z-index
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    resetModal();
                }
            });
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
    });

    createIncomeButton.addEventListener('click', () => {
        updateTypeSelection('Income');
        filterCategories('Income');
    });

    // Filter categories based on transaction type
    function filterCategories(type) {
        const options = transactionCategorySelect.querySelectorAll('option');

        options.forEach(option => {
            if (option.dataset.type === type || option.value === "") {
                option.style.display = 'block'; // Show relevant options
            } else {
                option.style.display = 'none'; // Hide irrelevant options
            }
        });

        transactionCategorySelect.value = ""; // Reset selection
    }

    // Populate modal fields with transaction data
    function populateTransactionData(transaction) {
        transactionForm.querySelector('input[name="TransactionId"]').value = transaction.transactionId; // Assuming Id is the primary key
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
                    Swal.fire({
                        title: 'Success',
                        text: 'Transaction created successfully!',
                        icon: 'success',
                        customClass: {
                            popup: 'swal2-front' // Custom class for z-index
                        }
                    }).then(() => {
                        resetModal();
                        window.location.reload();
                    });
                } else {
                    Swal.fire({
                        title: 'Error',
                        text: 'An error occurred while adding the transaction.',
                        icon: 'error',
                        customClass: {
                            popup: 'swal2-front' // Custom class for z-index
                        }
                    });
                }
            })
            .catch(error => {
                console.error('Error adding transaction:', error);
                Swal.fire({
                    title: 'Error',
                    text: error.message,
                    icon: 'error',
                    customClass: {
                        popup: 'swal2-front' // Custom class for z-index
                    }
                });
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
                    Swal.fire({
                        title: 'Success',
                        text: 'Transaction updated successfully!',
                        icon: 'success',
                        customClass: {
                            popup: 'swal2-front' // Custom class for z-index
                        }
                    }).then(() => {
                        resetModal();
                        window.location.reload();
                    });
                } else {
                    Swal.fire({
                        title: 'Error',
                        text: 'An error occurred while updating the transaction.',
                        icon: 'error',
                        customClass: {
                            popup: 'swal2-front' // Custom class for z-index
                        }
                    });
                }
            })
            .catch(error => {
                console.error('Error updating transaction:', error);
                Swal.fire({
                    title: 'Error',
                    text: error.message,
                    icon: 'error',
                    customClass: {
                        popup: 'swal2-front' // Custom class for z-index
                    }
                });
            });
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






    // Event listeners
    addTransactionButton.addEventListener('click', openAddTransactionModal);
    closeModalButton.addEventListener('click', closeAddTransactionModal);
    transactionForm.addEventListener('submit', currentTransactionId ? handleEditTransaction : handleAddTransaction);
    transactionForm.addEventListener('change', () => { isModalDirty = true; }); // Set dirty flag on change

    // Close modal when clicking outside of it
    window.addEventListener('click', (event) => {
        if (event.target === modal) { // Check if the click is on the modal background
            closeAddTransactionModal();
        }
    });
});
