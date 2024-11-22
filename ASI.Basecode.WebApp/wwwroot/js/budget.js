$(document).ready(function () {
    // Open modal for adding budget
    $('#addBudgetButton').on('click', function () {
        $('#budgetModal').removeClass('hidden');
        $('#modalTitle').text('Add Budget');
        $('#budgetForm')[0].reset(); // Reset the form fields
        $('#transactionCategory').val(''); // Clear dropdown
        $('#budgetAmount').val(''); // Clear input
        $('#budgetForm').data('edit', false); // Mark as 'add'
    });

    // Open modal for editing budget
    $(document).on('click', '.editBudgetButton', function () {
        const id = $(this).data('id');
        const category = $(this).data('category');
        const budget = $(this).data('budget');

        $('#budgetModal').removeClass('hidden');
        $('#modalTitle').text('Edit Budget');
        $('#transactionCategory').val(category);
        $('#budgetAmount').val(budget);
        $('#budgetForm').data('edit', true).data('id', id); // Mark as 'edit' with ID
    });

    // Close modal
    $('#cancelButton').on('click', function () {
        $('#budgetModal').addClass('hidden');
    });

    // Handle form submission
    $('#budgetForm').on('submit', function (e) {
        e.preventDefault();

        const isEdit = $('#budgetForm').data('edit');
        const data = {
            BudgetId: isEdit ? $('#budgetForm').data('id') : null, // Include ID for edit
            CategoryId: $('#transactionCategory').val(),
            MonthlyBudget: $('#budgetAmount').val(),
        };

        const url = isEdit ? '/Budget/UpdateBudget' : '/Budget/AddBudget'; // Different endpoint for add/edit

        $.ajax({
            url: url,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function (response) {
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: response.message,
                }).then(() => {
                    location.reload(); // Reload to reflect changes
                });
            },
            error: function (error) {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: error.responseJSON?.message || 'Failed to save budget. Please try again.',
                });
            }
        });
    });
});
