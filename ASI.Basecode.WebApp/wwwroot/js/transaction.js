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
        transactionCategorySelect.value = "";
        transactionAccountSelect.value = "";
        isModalDirty = false; // Reset the dirty flag
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

    // Filter categories based on transaction type
    function filterCategories(type) {
        const options = transactionCategorySelect.querySelectorAll('option');
        let hasCategories = false; // Track if any category is available

        options.forEach(option => {
            if (option.dataset.type === type || option.value === "") {
                option.style.display = 'block';
                if (option.value !== "") {
                    hasCategories = true;
                }
            } else {
                option.style.display = 'none';
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
                hasAccounts = true;
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

        // Send data to the server
        fetch('/Transaction/Index', { 
            method: 'POST',
            body: formData
        })
            .then(response => {
                if (!response.ok) {
                    return response.text().then(text => {
                        console.error('Error response:', text); // Log the error response for debugging
                        throw new Error('Network response was not ok');
                    });
                }
                return response.json();
            })
            .then(response => {
                if (response.success) {
                    // Add the new transaction row to the table
                    const newRow = document.createElement('tr');
                    newRow.innerHTML = `
                <td class="border px-4 py-2">${response.data.Category}</td>
                <td class="border px-4 py-2">${response.data.Description}</td>
                <td class="border px-4 py-2">₱ ${response.data.Amount}</td>
                <td class="border px-4 py-2">${response.data.Account}</td>
                <td class="border px-4 py-2">${response.data.Date}</td>
                <td class="border px-4 py-2">
                    <button class="bg-blue-500 text-white px-3 py-1 rounded-lg">Edit</button>
                    <button class="bg-red-500 text-white px-2 py-1 rounded-lg">Delete</button>
                </td>
            `;
                    document.getElementById('transactionTable').querySelector('tbody').appendChild(newRow);

                    // Close the modal
                    resetModal();

                    // Optionally display a success message
                    alert(response.message);
                } else {
                    // Handle errors returned from the server
                    alert(response.message);
                }
            })
            .catch(error => {
                // Handle network errors
                console.error('There was a problem with the fetch operation:', error);
                alert('An error occurred. Please try again later.');
            });
    });

});
