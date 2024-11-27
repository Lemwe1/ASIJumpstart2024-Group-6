document.addEventListener('DOMContentLoaded', function () {

    // Add modal elements
    const addBudgetModal = document.getElementById('addBudgetModal');
    const addForm = document.getElementById('budgetForm');
    const budgetAmountInput = document.getElementById('budgetAmount');

    // Edit modal elements
    const editBudgetModal = document.getElementById('editBudgetModal');
    const editForm = document.getElementById('editBudgetForm');
    const editBudgetAmountInput = document.getElementById('editBudgetAmount');

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

    // Apply the live validation to the budget amount input field
    validateBudgetInput(budgetAmountInput);
    validateBudgetInput(editBudgetAmountInput);

    // Initialize the flag
    let isModalDirty = false;

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

                // Check if the budget's last reset date is from the current month
                const lastResetDate = new Date(budget.LastResetDate);
                const currentMonth = new Date().getMonth();

                if (lastResetDate.getMonth() !== currentMonth) {
                    // Reset the remaining budget and update the last reset date
                    budget.RemainingBudget = budget.MonthlyBudget;
                    budget.LastResetDate = new Date().toISOString();  // Update to current date
                    await updateBudget(budget);
                    Swal.fire({
                        icon: 'info',
                        title: 'Budget Reset',
                        text: 'Your budget has been reset for the new month!',
                    });
                }

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

    // Function to update the budget after resetting the remaining budget
    async function updateBudget(budget) {
        try {
            const response = await fetch(`/Home/UpdateBudget/${budget.BudgetId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                },
                body: JSON.stringify({
                    BudgetId: budget.BudgetId,
                    CategoryId: budget.CategoryId,
                    MonthlyBudget: budget.MonthlyBudget,
                    RemainingBudget: budget.RemainingBudget,
                    UserId: budget.UserId,
                    LastResetDate: budget.LastResetDate  // Ensure LastResetDate is updated
                })
            });
            if (!response.ok) {
                throw new Error('Failed to update budget');
            }
        } catch (error) {
            console.error('Error updating budget:', error);
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'An error occurred while updating your budget.'
            });
        }
    }

    function resetAddFormFields() {
        document.getElementById('budgetAmount').value = '';
        document.getElementById('budgetCategory').value = '';
    }

    // Close add modal
    document.getElementById('closeBudgetModalBtn').addEventListener('click', function () {
        addBudgetModal.classList.add('hidden');
        document.body.classList.remove('overflow-hidden');
    });

    // Close edit modal
    document.getElementById('closeEditBudgetModalBtn').addEventListener('click', function () {
        editBudgetModal.classList.add('hidden');
        document.body.classList.remove('overflow-hidden');
    });

    // Close the modal if clicked outside the modal content
    addBudgetModal.addEventListener('click', function (event) {
        if (event.target === addBudgetModal) {
            addBudgetModal.classList.add('hidden');
            document.body.classList.remove('overflow-hidden');
        }
    });

    editBudgetModal.addEventListener('click', function (event) {
        if (event.target === editBudgetModal) {
            editBudgetModal.classList.add('hidden');
            document.body.classList.remove('overflow-hidden');
        }
    });

    function populateEditModalFields(budget) {
        const { budgetId, monthlyBudget, categoryId, lastResetDate } = budget;

        document.getElementById('budgetId').value = budgetId;
        // Ensure MonthlyBudget is a number before calling .toFixed()
        const formattedMonthlyBudget = isNaN(monthlyBudget) ? 0.00 : parseFloat(monthlyBudget).toFixed(2);
        document.getElementById('editBudgetAmount').value = formattedMonthlyBudget;
        document.getElementById('editBudgetCategory').value = categoryId || '';

        console.log("Budget data:", budget);
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

    // Add event listener to detect changes in the form
    const editBudgetForm = document.getElementById('editBudgetForm');
    editBudgetForm.addEventListener('input', () => {
        isModalDirty = true;  // Set flag to true when any input changes
    });

    editBudgetForm.addEventListener('submit', async (e) => {
        e.preventDefault();

        const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
        const token = tokenElement ? tokenElement.value : null;

        const budgetId = parseInt(document.getElementById('budgetId').value, 10);
        const categoryId = parseInt(document.getElementById('editBudgetCategory').value, 10);
        const monthlyBudget = parseFloat(document.getElementById('editBudgetAmount').value);
        const userId = document.getElementById('userId').value;

        // Check if the form is dirty (i.e., the user has made changes)
        if (!isModalDirty) {
            $('#editBudgetModal').modal('hide');
            window.location.reload();
            return;
        }

        // Prepare the data for the update
        const data = {
            BudgetId: budgetId,
            CategoryId: categoryId,
            MonthlyBudget: monthlyBudget,
            UserId: parseInt(userId, 10),
            LastResetDate: new Date().toISOString() 
        };

        try {
            const response = await fetch(`/Home/UpdateBudget/${budgetId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: JSON.stringify(data)
            });

            if (response.ok) {
                const result = await response.json();

                if (result.success) {
                    // Show success if the update was successful
                    Swal.fire({
                        title: 'Success',
                        text: 'Budget updated successfully!',
                        icon: 'success',
                        confirmButtonColor: '#3B82F6',
                        customClass: { popup: 'swal2-front' }
                    }).then(() => {
                        $('#editBudgetModal').modal('hide');  // Close the modal after success
                        window.location.reload();  // Optionally reload the page
                    });
                } else {
                    // Show error if the update failed
                    Swal.fire({
                        title: 'Error',
                        text: result.message || 'An error occurred while updating the budget.',
                        icon: 'error',
                        customClass: { popup: 'swal2-front' }
                    });
                }
            } else {
                // Handle non-OK HTTP responses
                const text = await response.text();
                console.error('Error Response Text:', text);
                Swal.fire({
                    title: 'Error',
                    text: 'Failed to update budget. Please try again.',
                    icon: 'error',
                    customClass: { popup: 'swal2-front' }
                });
            }
        } catch (error) {
            // Handle unexpected errors
            Swal.fire({
                title: 'Error',
                text: error.message || 'An unexpected error occurred.',
                icon: 'error',
                customClass: { popup: 'swal2-front' }
            });
        }
    });
});
