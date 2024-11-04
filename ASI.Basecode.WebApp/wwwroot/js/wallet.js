// Modal Elements
const addAccountModal = document.getElementById('addAccountModal');
const addAccountButton = document.getElementById('addAccountButton');
const modalContent = document.getElementById('modalContent');

// Form field elements
const addFormFields = document.getElementById('addFormFields');

// Close buttons for the modals
const closeAddModal = document.getElementById('closeAddModal');
const closeEditModal = document.getElementById('closeEditModal');

// Arrays to store accounts (can be removed if stored in the DB)
let debitAccounts = [];

document.addEventListener('DOMContentLoaded', () => {
    const totalBalanceElement = document.querySelector('#totalBalance .balance-value');
    const toggleTotalBalanceButton = document.getElementById('toggleBalanceVisibility');

    // Store the original total balance for toggling
    const originalTotalBalance = totalBalanceElement.textContent.replace('₱', '');

    // Toggle total balance visibility
    toggleTotalBalanceButton.addEventListener('click', () => {
        const isVisible = totalBalanceElement.dataset.visible === 'true';
        if (isVisible) {
            totalBalanceElement.textContent = '₱' + '*'.repeat(originalTotalBalance.length - 1); // Replace with asterisks
            toggleTotalBalanceButton.classList.remove('fa-eye');
            toggleTotalBalanceButton.classList.add('fa-eye-slash');
            totalBalanceElement.dataset.visible = 'false'; // Update visibility state
        } else {
            totalBalanceElement.textContent = `₱${originalTotalBalance}`; // Show the original balance
            toggleTotalBalanceButton.classList.remove('fa-eye-slash');
            toggleTotalBalanceButton.classList.add('fa-eye');
            totalBalanceElement.dataset.visible = 'true'; // Update visibility state
        }
    });

    // Add event listeners to all eye icons for individual balances
    const toggleButtons = document.querySelectorAll('.toggle-balance');

    toggleButtons.forEach(button => {
        const accountId = button.dataset.accountId;
        const balanceElement = button.previousElementSibling; // The balance span

        // Store the original balance for toggling
        balanceElement.dataset.originalBalance = balanceElement.textContent.replace('₱', '');

        button.addEventListener('click', () => {
            const isVisible = balanceElement.dataset.visible === 'true';

            if (isVisible) {
                balanceElement.textContent = '₱' + '*'.repeat(balanceElement.dataset.originalBalance.length); // Replace balance with asterisks
                button.classList.remove('fa-eye');
                button.classList.add('fa-eye-slash');
                balanceElement.dataset.visible = 'false'; // Update visibility state
            } else {
                balanceElement.textContent = `₱${balanceElement.dataset.originalBalance}`; // Show the original balance
                button.classList.remove('fa-eye-slash');
                button.classList.add('fa-eye');
                balanceElement.dataset.visible = 'true'; // Update visibility state
            }
        });
    });
});



// Function to update the total balance displayed
function updateNetWorth() {
    // Calculate the total debit and total liabilities
    let totalDebit = debitAccounts.reduce((acc, account) => acc + account.WalletBalance, 0);

    // Update the net worth section in the HTML
    document.getElementById('balanceSection').innerHTML = `
        <h2 id="totalBalance" class="text-2xl font-bold" data-visible="true">Total Balance: ₱${totalDebit.toFixed(2)}</h2>
    `;
}


// Function to reset the Add Account form fields
function resetAddFormFields() {
    document.getElementById('accountName').value = '';
    document.getElementById('accountBalance').value = '';
    document.getElementById('createIcon').value = '';
    document.getElementById('accountColor').value = '#000000'; // Default color
}

// Function to show confirmation dialog
async function showConfirmationDialog(modal, resetFunction) {
    const { isConfirmed } = await Swal.fire({
        title: 'Are you sure?',
        text: 'Do you want to discard them?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#a6a6a6',
        confirmButtonText: 'Yes, discard it!',
        cancelButtonText: 'No, cancel!'
    });

    if (isConfirmed) {
        closeModal(modal);
        if (resetFunction) {
            resetFunction(); 
        }
    }
}

// Close button for Add Account Modal
if (closeAddModal) {
    closeAddModal.addEventListener('click', () => {
        showConfirmationDialog(addAccountModal, resetAddFormFields); // Show confirmation and reset form
    });
}

// Close button for Edit Account Modal
if (closeEditModal) {
    closeEditModal.addEventListener('click', () => {
        showConfirmationDialog(editAccountModal, null); // Show confirmation, no reset needed
    });
}

// Function to open modal
function openModal(modal) {
    if (modal) {
        modal.classList.remove('hidden');
    } else {
        Swal.fire({
            title: 'Error',
            text: result.message || 'An error occurred.',
            icon: 'error',
            customClass: { popup: 'swal2-front' }
        });
    }
}

// Function to close modal
function closeModal(modal) {
    if (modal) {
        modal.classList.add('hidden');
    } else {
        Swal.fire({
            title: 'Error',
            text: result.message || 'An error occurred.',
            icon: 'error',
            customClass: { popup: 'swal2-front' }
        });
    }
}

// Open the modal when the Add Account button is clicked
if (addAccountButton) {
    addAccountButton.addEventListener('click', () => {
        openModal(addAccountModal);
    });
} else {
    Swal.fire({
        title: 'Error',
        text: result.message || 'An error occurred.',
        icon: 'error',
        customClass: { popup: 'swal2-front' }
    });
}

// Close the modal if the background (not the modal content) is clicked
if (addAccountModal) {
    addAccountModal.addEventListener('click', (e) => {
        if (e.target === addAccountModal) { // Only close if clicked outside modal content
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
                    closeModal(addAccountModal);
                    resetAddFormFields();
                }
            });
        }
    });
} else {
    Swal.fire({
        title: 'Error',
        text: result.message || 'An error occurred.',
        icon: 'error',
        customClass: { popup: 'swal2-front' }
    });
}

// Function to load Debit form fields
function loadDebitForm() {
    if (addFormFields) {
        addFormFields.innerHTML = `
            <div class="mb-4">
                <label class="block">Name</label>
                <input type="text" id="accountName" class="border p-2 w-full" required />
            </div>
            <div class="mb-4">
                <label class="block">Balance</label>
                <input type="number" id="accountBalance" class="border p-2 w-full" required step="any" />
            </div>
            <div class="mb-6">
                <label for="createIcon" class="block text-sm font-medium text-gray-700">Icon</label>
                <select name="Icon" id="createIcon" class="mt-1 block w-full px-4 py-2 border rounded-md bg-white text-gray-900 focus:ring-blue-500 focus:border-blue-500" required>
                    <option value="">Select Icon</option>
                    <option value="fas fa-apple-alt">Apple (Food)</option>
                    <option value="fas fa-shopping-bag">Shopping Bag</option>
                    <option value="fas fa-bolt">Bolt (Electricity)</option>
                    <option value="fas fa-wallet">Wallet (Income)</option>
                    <option value="fas fa-money-bill-wave">Money (Expense)</option>
                </select>
            </div>
            <div class="mb-4">
                <label class="block">Color</label>
                <input type="color" id="accountColor" class="border p-2 w-full" style="height: 50px;" required />
            </div>
        `;
    } else {
        console.error('Add Form Fields element not found.');
    }
}



// Handle form submission for adding an account
document.getElementById('addAccountForm').addEventListener('submit', async (e) => {
    e.preventDefault();

    const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
    const token = tokenElement ? tokenElement.value : null;

    const name = document.getElementById('accountName').value;
    const color = document.getElementById('accountColor').value;
    const icon = document.getElementById('createIcon').value;

    let data = {
        WalletIcon: icon,
        WalletColor: color,
        WalletBalance: 0,
        WalletName: name
    };

    const balance = parseFloat(document.getElementById('accountBalance').value);
    data.WalletBalance = balance;

    console.log('Sending data:', JSON.stringify(data));

    try {
        const response = await fetch('/Wallet/Create', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify(data)
        });

        const result = await response.json();
        if (result.success) {
            closeModal(addAccountModal);
            Swal.fire({
                title: 'Success',
                text: 'Wallet Created successfully!',
                icon: 'success',
                confirmButtonColor: '#3B82F6',
                customClass: { popup: 'swal2-front' }
            }).then(() => {
                loadAccounts();
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
    } catch (error) {
        Swal.fire({
            title: 'Error',
            text: result.message || 'An error occurred.',
            icon: 'error',
            customClass: { popup: 'swal2-front' }
        });
    }
});

// Function to load accounts from the server and render them
async function loadAccounts() {
    try {
        const response = await fetch('/Wallet'); // Adjust URL if necessary
        const accounts = await response.json();

        debitAccounts = accounts.filter(account => account.DeLiType === 'debit');

        // Render the updated accounts
        renderAccounts();
    } catch (error) {
        Swal.fire({
            title: 'Error',
            text: result.message || 'An error occurred.',
            icon: 'error',
            customClass: { popup: 'swal2-front' }
        });
        Swal.fire({
            title: 'Error',
            text: result.message || 'An error occurred.',
            icon: 'error',
            customClass: { popup: 'swal2-front' }
        });
    }
}

function openEditModal(account) {
    // Ensure account is defined
    if (!account) {
        console.error('No account data provided.');
        return;
    }

    // Populate fields in the edit modal
    document.getElementById('editAccountId').value = account.WalletId || ''; // Default to empty string if undefined
    document.getElementById('editAccountName').value = account.WalletName || ''; // Default to empty string if undefined
    document.getElementById('editAccountBalance').value = account.WalletBalance || ''; // Default to empty string if undefined
    document.getElementById('editAccountColor').value = account.WalletColor || '#000000'; // Default color if undefined
    document.getElementById('editCreateIcon').value = account.WalletIcon || ''; // Default to empty string if undefined

    // Make sure the balance input accepts decimals
    const balanceInput = document.getElementById('editAccountBalance');
    balanceInput.setAttribute('step', 'any'); // Allow decimal inputs

    // Ensure the icon selection is required
    const iconSelect = document.getElementById('editCreateIcon');
    iconSelect.setAttribute('required', 'required'); // Make icon selection required

    openModal(editAccountModal);
}


// Close the edit modal if the background (not the modal content) is clicked
if (editAccountModal) {
    editAccountModal.addEventListener('click', (e) => {
        if (e.target === editAccountModal) { // Only close if clicked outside modal content
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
                    closeModal(editAccountModal)
                };
            });
        }
    });
} else {
    Swal.fire({
        title: 'Error',
        text: result.message || 'An error occurred.',
        icon: 'error',
        customClass: { popup: 'swal2-front' }
    });
    Swal.fire({
    title: 'Error',
    text: result.message || 'An error occurred.',
    icon: 'error',
    customClass: { popup: 'swal2-front' }
});
}

// Handle form submission for editing an account
document.getElementById('editAccountForm').addEventListener('submit', async (e) => {
    e.preventDefault();

    const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
    const token = tokenElement ? tokenElement.value : null;

    const id = document.getElementById('editAccountId').value;
    const name = document.getElementById('editAccountName').value;
    const color = document.getElementById('editAccountColor').value;
    const icon = document.getElementById('editCreateIcon').value;

    let data = {
        WalletId: id,
        WalletIcon: icon,
        WalletColor: color,
        WalletName: name,
        WalletBalance: 0 // Set this dynamically for debit or borrowed
    };

    const balance = parseFloat(document.getElementById('editAccountBalance').value);
    data.WalletBalance = balance;

    console.log('Sending data:', JSON.stringify(data));

    const { isConfirmed } = await Swal.fire({
        title: 'Are you sure?',
        text: 'Do you really want to edit this walet?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#a6a6a6',
        confirmButtonText: 'Yes, edit it!',
        cancelButtonText: 'No, cancel!'
    });
    if (isConfirmed) {
        try {
            const response = await fetch(`/Wallet/Edit/${id}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: JSON.stringify(data)
            });

            const result = await response.json();
            if (result.success) {
                closeModal(editAccountModal);
                Swal.fire({
                    title: 'Success',
                    text: 'Wallet Edited successfully!',
                    icon: 'success',
                    confirmButtonColor: '#3B82F6',
                    customClass: { popup: 'swal2-front' }
                }).then(() => {
                    loadAccounts();
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
        } catch (error) {
            Swal.fire({
                title: 'Error',
                text: result.message || 'An error occurred.',
                icon: 'error',
                customClass: { popup: 'swal2-front' }
            });
        }
    }
});

// Handle account deletion
document.getElementById('deleteAccountButton').addEventListener('click', async () => {
    const id = document.getElementById('editAccountId').value;

    if (!id) {
        console.error('No account ID specified for deletion.');
        return;
    }

    const { isConfirmed } = await Swal.fire({
        title: 'Are you sure?',
        text: 'Do you really want to delete this Wallet?',
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
            const response = await fetch(`/Wallet/Delete/${id}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                }
            });

            const result = await response.json();
            if (result.success) {
                closeModal(editAccountModal);
                Swal.fire({
                    title: 'Success',
                    text: 'Wallet Deleted successfully!',
                    icon: 'success',
                    confirmButtonColor: '#3B82F6',
                    customClass: { popup: 'swal2-front' }
                }).then(() => {
                    loadAccounts();
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
        } catch (error) {
            Swal.fire({
                title: 'Error',
                text: result.message || 'An error occurred.',
                icon: 'error',
                customClass: { popup: 'swal2-front' }
            });
        }
    }
    
});

// Load accounts on page load
document.addEventListener('DOMContentLoaded', () => {
    loadAccounts();
    loadDebitForm(); // Load the form fields initially
});
