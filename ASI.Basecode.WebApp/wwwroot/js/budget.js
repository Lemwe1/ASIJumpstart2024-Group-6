$(document).ready(function () {
    // Open modal for adding budget
    $('#addBudgetButton').on('click', function () {
        $('#budgetModal').removeClass('hidden'); // Show the modal
        $('#modalTitle').text('Add Budget'); // Set the modal title to "Add Budget"
        $('#budgetForm')[0].reset(); // Reset the form fields
        $('#transactionCategory').val(''); // Clear the category dropdown
        $('#budgetAmount').val(''); // Clear the input field for budget amount
        $('#budgetForm').data('edit', false); // Mark as 'add' (not editing)
    });

    // Open modal for editing budget
    $(document).on('click', '.editBudgetButton', function () {
        const id = $(this).data('id');
        const category = $(this).data('category');
        const budget = $(this).data('budget');

        $('#budgetModal').removeClass('hidden'); // Show the modal
        $('#modalTitle').text('Edit Budget'); // Set the modal title to "Edit Budget"
        $('#transactionCategory').val(category); // Set the category in the dropdown
        $('#budgetAmount').val(budget); // Set the budget amount
        $('#budgetForm').data('edit', true).data('id', id); // Mark as 'edit' and store the budget ID
    });

    // Close modal
    $('#cancelButton').on('click', function () {
        $('#budgetModal').addClass('hidden'); // Hide the modal when clicking Cancel
    });

    // Handle form submission
    $('#budgetForm').on('submit', function (e) {
        e.preventDefault(); // Prevent default form submission

        const isEdit = $('#budgetForm').data('edit'); // Check if we're editing
        const userId = $('#userId').val(); // Get the UserId from the hidden input field
        const data = {
            BudgetId: isEdit ? $('#budgetForm').data('id') : null, // Use ID if editing
            CategoryId: parseInt($('#transactionCategory').val(), 10), // Ensure CategoryId is an integer
            MonthlyBudget: parseFloat($('#budgetAmount').val()), // Ensure MonthlyBudget is a float
            UserId: parseInt(userId, 10) // Ensure UserId is an integer
        };

        console.log('Payload:', data); // Log the payload to check the data being sent

        const url = isEdit ? '/Home/UpdateBudget' : '/Home/AddBudget'; // Choose the correct API endpoint based on whether it's an edit or add
        const method = isEdit ? 'PUT' : 'POST'; // Use PUT for editing, POST for adding

        // Send the AJAX request
        $.ajax({
            headers: {
                "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() // Include the CSRF token
            },
            url: url,
            type: method,
            contentType: 'application/json',
            data: JSON.stringify(data), // Send data as a JSON string
            success: function (response) {
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: response.message, // Show success message
                }).then(() => {
                    location.reload(); // Reload the page to reflect changes
                });
            },
            error: function (error) {
                console.error('Full Error Response:', error); // Log full error response for debugging
                let errorMessage = 'An unknown error occurred. Please try again.';
                if (error.responseJSON) {
                    errorMessage = error.responseJSON.message || error.responseJSON.errors?.join(', ') || errorMessage;
                }
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: errorMessage, // Display error message
                });
            }
        });
    });
});
