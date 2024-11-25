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


    // Handle form submission for editing a budget
    document.getElementById('editBudgetForm').addEventListener('submit', async (e) => {
        e.preventDefault();

        const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
        const token = tokenElement ? tokenElement.value : null;

        const budgetId = parseInt(document.getElementById('budgetId').value, 10); 
        const categoryId = parseInt(document.getElementById('editBudgetCategory').value, 10);
        const monthlyBudget = parseFloat(document.getElementById('editBudgetAmount').value); 

        console.log('Budget ID being sent:', budgetId);

        const data = {
            BudgetId: budgetId, // Matches backend model property name
            CategoryId: categoryId,
            MonthlyBudget: monthlyBudget
        
        };

        console.log('Sending data:', JSON.stringify(data));

        const { isConfirmed } = await Swal.fire({
            title: 'Are you sure?',
            text: 'Do you really want to edit this budget?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#a6a6a6',
            confirmButtonText: 'Yes, edit it!',
            cancelButtonText: 'No, cancel!'
        });

        if (isConfirmed) {
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
                        Swal.fire({
                            title: 'Success',
                            text: 'Budget updated successfully!',
                            icon: 'success',
                            confirmButtonColor: '#3B82F6',
                            customClass: { popup: 'swal2-front' }
                        }).then(() => {
                            window.location.reload();
                        });
                    } else {
                        Swal.fire({
                            title: 'Error',
                            text: result.message || 'An error occurred while updating the budget.',
                            icon: 'error',
                            customClass: { popup: 'swal2-front' }
                        });
                    }
                } else {
                    // Debug non-OK HTTP responses
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
        }
    });


    document.getElementById('deleteBudgetButton').addEventListener('click', async function () {
        const id = document.getElementById('budgetId').value;
        console.log('Attempting to delete budget with ID:', id);

        if (!id) {
            Swal.fire('Error', 'Category ID is missing.', 'error');
            return;
        }

        Swal.fire({
            title: 'Are you sure?',
            text: 'You won\'t be able to revert this!',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#e53e3e',
            cancelButtonColor: '#718096',
            confirmButtonText: 'Yes, delete it!'
        }).then(async (result) => {
            if (result.isConfirmed) {
                const formData = new FormData();
                formData.append('__RequestVerificationToken', document.querySelector('#editBudgetForm input[name="__RequestVerificationToken"]').value);
                console.log('Form data for deletion:', [...formData]);

                try {
                    const response = await fetch(`/Home/DeleteBudget/${id}`, {
                        method: 'POST',
                        body: formData
                    });
                    console.log('Response from delete budget:', response);

                    if (response.ok) {
                        const result = await response.json();
                        if (result.success) {
                            
                            editBudgetModal.classList.add('hidden');
                            editBudgetModal.classList.remove('flex');

                            Swal.fire('Deleted!', 'Budget has been deleted.', 'success').then(() => {
                                window.location.reload();
                            });
                        } else {
                            Swal.fire('Error', result.message || 'Failed to delete budget.', 'error');
                        }
                    } else {
                        const result = await response.json();
                        Swal.fire('Error', result.message || 'Failed to delete budget.', 'error');
                    }
                } catch (error) {
                    console.error('Error deleting budget:', error);
                    Swal.fire('Error', 'Error occurred while deleting the budget.', 'error');
                }
            }
        });
    });

});
