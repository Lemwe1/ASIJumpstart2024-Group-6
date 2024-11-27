document.addEventListener('DOMContentLoaded', function () {

    // Add modal elements
    const addBudgetModal = document.getElementById('addBudgetModal');
    const addForm = document.getElementById('budgetForm');
    const budgetAmountInput = document.getElementById('budgetAmount');
    const budgetCategoryInput = document.getElementById('budgetCategory');

    // Edit modal elements
    const editBudgetModal = document.getElementById('editBudgetModal');
    const editForm = document.getElementById('editBudgetForm');
    const editBudgetAmountInput = document.getElementById('editBudgetAmount');
    const editBudgetCategoryInput = document.getElementById('editBudgetCategory');

    // Initialize the flag
    let isModalDirty = false;

    // Live validation for the budget input
    function validateBudgetInput(inputElement) {
        inputElement.addEventListener('input', function () {
            let amount = parseFloat(inputElement.value);  // Directly parse the input value

            // If the input is not a valid number, treat amount as 0
            if (isNaN(amount)) {
                amount = 0;
            }

            // Check if the amount exceeds the maximum budget limit
            if (amount > 1_000_000_000_000) {
                // Show SweetAlert error message
                Swal.fire({
                    title: 'Validation Error',
                    text: `Budget must not exceed 1 trillion.`,
                    icon: 'warning',
                    confirmButtonText: 'OK',
                    confirmButtonColor: '#3B82F6',
                });

                // Reset the input field if the value exceeds the limit
                inputElement.value = '';
            }
        });
    }

    // Apply the live validation to the budget amount input fields
    validateBudgetInput(budgetAmountInput);
    validateBudgetInput(editBudgetAmountInput);

    // Function to open the Add Budget modal
    function openAddBudgetModal() {
        addBudgetModal.classList.remove('hidden');
        addBudgetModal.classList.add('flex');
        document.body.classList.add('overflow-hidden');
        resetAddFormFields();
    }

    // Function to open the Edit Budget modal
    function openEditBudgetModal(budget) {
        editBudgetModal.classList.remove('hidden');
        editBudgetModal.classList.add('flex');
        document.body.classList.add('overflow-hidden');
        populateEditModalFields(budget);
    }

    // Open modal for adding budget
    document.getElementById('addBudgetButton').addEventListener('click', function () {
        openAddBudgetModal();
    });

    // Edit budget buttons
    document.querySelectorAll('.editBudgetButton').forEach(button => {
        button.addEventListener('click', () => {
            const budgetId = button.getAttribute('data-id');
            fetchBudget(budgetId);
        });
    });

    // Fetch budget for editing
    async function fetchBudget(budgetId) {
        try {
            const response = await fetch(`/Home/GetBudget/${budgetId}`);
            const result = await response.json();

            if (result.success) {
                const budget = result.data;

                openEditBudgetModal(budget);
            } else {
                console.error('Error fetching budget:', result.message);
                alert(result.message);
            }
        } catch (error) {
            console.error('Error fetching budget:', error);
            alert('Error fetching budget data.');
        }
    }

    // Handle form submission for adding a budget
    function handleBudgetFormSubmit(e) {
        e.preventDefault();

        const userId = document.getElementById('userId').value;
        const data = {
            BudgetId: null, // Null for adding
            CategoryId: parseInt(document.getElementById('budgetCategory').value, 10),
            MonthlyBudget: parseFloat(document.getElementById('budgetAmount').value),
            UserId: parseInt(userId, 10),
            LastResetDate: new Date().toISOString()  // Add LastResetDate to the payload
        };

        fetch('/Home/AddBudget', {
            method: 'POST',
            headers: {
                "Content-Type": "application/json",
                "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify(data)
        })
            .then(response => response.json())
            .then(data => {
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: data.message
                }).then(() => {
                    location.reload();
                });
            })
            .catch(error => {
                console.error('Error:', error);
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: error.message || 'An unknown error occurred. Please try again.'
                });
            });
    }

    // Handle add form submission
    document.getElementById('addBudgetForm').addEventListener('submit', handleBudgetFormSubmit);

    // Add event listener to detect changes in the form (for Edit modal)
    const editBudgetForm = document.getElementById('editBudgetForm');
    editBudgetForm.addEventListener('input', () => {
        isModalDirty = true;  // Set flag to true when any input changes
    });

    // Add event listener for reset button (reset the Add Budget form)
    document.getElementById('resetBudgetButton').addEventListener('click', function () {
        resetAddFormFields();
        isModalDirty = false;
    });

    // Reset the Add Budget form fields
    function resetAddFormFields() {
        document.getElementById('budgetAmount').value = '';
        document.getElementById('budgetCategory').value = '';
        isModalDirty = false; // Reset dirty flag on reset
    }

    // Populate edit modal fields
    function populateEditModalFields(budget) {
        const { budgetId, monthlyBudget, categoryId, lastResetDate } = budget;

        document.getElementById('budgetId').value = budgetId;
        // Ensure MonthlyBudget is a number before calling .toFixed()
        const formattedMonthlyBudget = isNaN(monthlyBudget) ? 0.00 : parseFloat(monthlyBudget).toFixed(2);
        document.getElementById('editBudgetAmount').value = formattedMonthlyBudget;
        document.getElementById('editBudgetCategory').value = categoryId || '';
    }

    // Close add modal with Swal confirmation
    addBudgetModal.addEventListener('click', function (event) {
        if (event.target === addBudgetModal) {
            if (isModalDirty || budgetAmountInput.value.trim() !== "" || budgetCategoryInput.value.trim() !== "") {
                Swal.fire({
                    title: 'Are you sure?',
                    text: 'You have unsaved changes. Do you really want to close without saving?',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes, Close',
                    cancelButtonText: 'No, Stay'
                }).then((result) => {
                    if (result.isConfirmed) {
                        addBudgetModal.classList.add('hidden');
                        document.body.classList.remove('overflow-hidden');
                    }
                });
            } else {
                addBudgetModal.classList.add('hidden');
                document.body.classList.remove('overflow-hidden');
            }
        }
    });

    // Close edit modal with Swal confirmation
    editBudgetModal.addEventListener('click', function (event) {
        if (event.target === editBudgetModal) {
            if (isModalDirty) {
                Swal.fire({
                    title: 'Are you sure?',
                    text: 'You have unsaved changes. Do you really want to close without saving?',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes, Close',
                    cancelButtonText: 'No, Stay'
                }).then((result) => {
                    if (result.isConfirmed) {
                        editBudgetModal.classList.add('hidden');
                        document.body.classList.remove('overflow-hidden');
                    }
                });
            } else {
                editBudgetModal.classList.add('hidden');
                document.body.classList.remove('overflow-hidden');
            }
        }
    });

    // Add event listener to close the "Add Budget" modal when clicking the close button
    document.getElementById('closeBudgetModalBtn').addEventListener('click', function () {
        if (isModalDirty || budgetAmountInput.value.trim() !== "" || budgetCategoryInput.value.trim() !== "") {
            Swal.fire({
                title: 'Are you sure?',
                text: 'You have unsaved changes. Do you really want to close without saving?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes, Close',
                cancelButtonText: 'No, Stay'
            }).then((result) => {
                if (result.isConfirmed) {
                    addBudgetModal.classList.add('hidden');
                    document.body.classList.remove('overflow-hidden');
                }
            });
        } else {
            addBudgetModal.classList.add('hidden');
            document.body.classList.remove('overflow-hidden');
        }
    });

    // Add event listener to close the "Edit Budget" modal when clicking the close button
    document.getElementById('closeEditBudgetModalBtn').addEventListener('click', function () {
        if (isModalDirty) {
            Swal.fire({
                title: 'Are you sure?',
                text: 'You have unsaved changes. Do you really want to close without saving?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes, Close',
                cancelButtonText: 'No, Stay'
            }).then((result) => {
                if (result.isConfirmed) {
                    editBudgetModal.classList.add('hidden');
                    document.body.classList.remove('overflow-hidden');
                }
            });
        } else {
            editBudgetModal.classList.add('hidden');
            document.body.classList.remove('overflow-hidden');
        }
    });

});
