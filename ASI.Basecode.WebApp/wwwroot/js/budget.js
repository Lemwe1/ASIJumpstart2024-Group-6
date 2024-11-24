document.addEventListener('DOMContentLoaded', function () {

    // Add modal elements
    const addBudgetModal = document.getElementById('addBudgetModal');
    const addForm = document.getElementById('budgetForm');

    // Edit modal elements
    const editBudgetModal = document.getElementById('editBudgetModal');
    const editForm = document.getElementById('editBudgetForm');

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

    function populateEditModalFields(budget) {
        const { budgetId, monthlyBudget, categoryId } = budget;

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
            UserId: parseInt(userId, 10)
        };

        console.log('Add Budget Payload:', data);

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

    // Handle budget deletion
    document.getElementById('deleteBudgetButton').addEventListener('click', async function () {
        const id = parseInt(document.getElementById('budgetId').value, 10);
        console.log("Budget ID to be deleted:", id);

        if (isNaN(id) || id <= 0) {
            console.error('Invalid budget ID specified for deletion.');
            Swal.fire({
                title: 'Error',
                text: 'Invalid budget ID. Please try again.',
                icon: 'error',
                confirmButtonColor: '#3B82F6'
            });
            return;
        }

        const { isConfirmed } = await Swal.fire({
            title: 'Are you sure?',
            text: 'Do you really want to delete this budget?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#a6a6a6',
            confirmButtonText: 'Yes, delete it!',
            cancelButtonText: 'No, cancel!'
        });

        if (isConfirmed) {
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

            if (!token) {
                console.error('Request verification token missing.');
                Swal.fire({
                    title: 'Error',
                    text: 'Session expired. Please refresh the page and try again.',
                    icon: 'error',
                    confirmButtonColor: '#3B82F6'
                });
                return;
            }

            try {
                const response = await fetch('/Home/DeleteBudget', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': token
                    },
                    body: JSON.stringify({ id: id }) // Send the budget ID in the request body
                });

                const result = await response.json();

                if (response.ok && result.success) {
                    Swal.fire({
                        title: 'Success',
                        text: result.message,
                        icon: 'success',
                        confirmButtonColor: '#3B82F6'
                    }).then(() => {
                        location.reload(); // Reload to reflect the change
                    });
                } else {
                    Swal.fire({
                        title: 'Error',
                        text: result.message || 'An error occurred during deletion.',
                        icon: 'error',
                        confirmButtonColor: '#3B82F6'
                    });
                }
            } catch (error) {
                console.error('Error deleting budget:', error);
                Swal.fire({
                    title: 'Error',
                    text: 'There was an issue deleting the budget. Please try again.',
                    icon: 'error',
                    confirmButtonColor: '#3B82F6'
                });
            }
        }
    });


    document.getElementById('editBudgetForm').addEventListener('submit', function (e) {
        const budgetId = document.getElementById('budgetId').value;
        console.log('Budget ID being sent:', budgetId);
    });

});
