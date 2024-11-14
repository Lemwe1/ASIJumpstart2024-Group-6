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
    const totalBalanceElement = document.getElementById('totalBalance');
    const toggleTotalBalanceButton = document.getElementById('toggleBalanceVisibility');
    const totalBalanceKey = 'totalBalanceVisibility';

    // Retrieve saved visibility state for total balance
    const isTotalBalanceVisible = localStorage.getItem(totalBalanceKey) === 'true';
    updateBalanceVisibility(totalBalanceElement, toggleTotalBalanceButton, isTotalBalanceVisible);

    toggleTotalBalanceButton.addEventListener('click', () => {
        const isVisible = totalBalanceElement.dataset.visible === 'true';
        updateBalanceVisibility(totalBalanceElement, toggleTotalBalanceButton, !isVisible);
        localStorage.setItem(totalBalanceKey, !isVisible); // Save new visibility state
    });

    // Function to update visibility and icon for a balance element
    function updateBalanceVisibility(balanceElement, toggleButton, isVisible) {
        const originalBalance = balanceElement.dataset.originalBalance || balanceElement.textContent.replace('₱', '').trim();
        if (isVisible) {
            balanceElement.textContent = `₱${originalBalance}`; // Show original balance
            toggleButton.classList.remove('fa-eye-slash');
            toggleButton.classList.add('fa-eye');
            balanceElement.dataset.visible = 'true';
        } else {
            balanceElement.textContent = '₱' + '*'.repeat(originalBalance.length); // Replace with asterisks
            toggleButton.classList.remove('fa-eye');
            toggleButton.classList.add('fa-eye-slash');
            balanceElement.dataset.visible = 'false';
        }
        balanceElement.dataset.originalBalance = originalBalance; // Store for future toggling
    }

    // Individual wallet balances
    const toggleButtons = document.querySelectorAll('.toggle-balance');

    toggleButtons.forEach(button => {
        const accountId = button.dataset.accountId;
        const balanceElement = button.previousElementSibling;
        const balanceKey = `walletBalanceVisibility_${accountId}`;
        const isBalanceVisible = localStorage.getItem(balanceKey) === 'true';

        // Set initial visibility based on localStorage
        updateBalanceVisibility(balanceElement, button, isBalanceVisible);

        button.addEventListener('click', () => {
            const isVisible = balanceElement.dataset.visible === 'true';
            updateBalanceVisibility(balanceElement, button, !isVisible);
            localStorage.setItem(balanceKey, !isVisible); // Save new visibility state
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


function resetAddFormFields() {
    document.getElementById('accountName').value = '';
    document.getElementById('accountBalance').value = '';
    document.getElementById('createIcon').value = "";
    document.getElementById('accountColor').value = '#000000';
}

document.getElementById('resetWalletButton').addEventListener('click', resetAddFormFields);


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
        document.body.classList.add('overflow-hidden'); 
        document.getElementById('createIcon').value = "";
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
        document.body.classList.remove('overflow-hidden'); 
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

function loadDebitForm() {
    if (addFormFields) {
        addFormFields.innerHTML = `
            <div class="mb-4">
                <label class="block dark:text-gray-300">Name</label>
                <input type="text" id="accountName" class="border p-2 w-full dark:bg-gray-700 dark:text-white" placeholder="Enter wallet name" required />
            </div>
            <div class="mb-4">
                <label class="block dark:text-gray-300">Balance</label>
                <input type="number" id="accountBalance" class="border p-2 w-full dark:bg-gray-700 dark:text-white" placeholder="0.00" required step="any" />
            </div>
            <div class="mb-6">
                <label for="createIcon" class="block text-sm font-medium text-gray-700 dark:text-gray-300">Icon</label>
                <select name="Icon" id="createIcon" class="mt-1 block w-full px-4 py-2 border rounded-md dark:bg-gray-700 dark:text-gray-300 text-gray-900 focus:ring-blue-500 focus:border-blue-500 " required>
                    <option value="" disabled>Select Icon</option>
                    <option value="🏦">🏦 Bank</option>
                    <option value="🏧">🏧 ATM </option>
                    <option value="💳">💳 Wallet</option>
                    <option value="💰">💰 Money</option>
                </select>
            </div>
            <div class="mb-4">
                <label class="block dark:text-gray-300">Color</label>
                <input type="color" id="accountColor" class="border mt-2 p-2 w-full dark:bg-gray-700" style="height: 50px;" title="Choose a color" required />
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

    // Preliminary check for existing wallet
    try {
        const existingWalletResponse = await fetch(`/Wallet/Exists?name=${encodeURIComponent(name)}`);
        const existingWalletResult = await existingWalletResponse.json();

        if (existingWalletResult.exists) {
            Swal.fire({
                title: 'Error',
                text: 'A wallet with this name already exists. Please choose a different name.',
                icon: 'error',
                customClass: { popup: 'swal2-front' }
            });
            return; // Stop the process if wallet already exists
        }

        let data = {
            WalletIcon: icon,
            WalletColor: color,
            WalletBalance: 0,
            WalletName: name
        };

        const balance = parseFloat(document.getElementById('accountBalance').value);
        data.WalletBalance = balance;

        console.log('Sending data:', JSON.stringify(data));

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
        console.error('Error during wallet creation:', error);
        Swal.fire({
            title: 'Error',
            text: 'An unexpected error occurred. Please try again later.',
            icon: 'error',
            customClass: { popup: 'swal2-front' }
        });
    }
});

let oldBalance = 0;

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

    oldBalance = account.WalletBalance;

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
        WalletBalance: 0 // Placeholder for balance, to be updated
    };

    const balance = parseFloat(document.getElementById('editAccountBalance').value);
    data.WalletBalance = balance;

    console.log('Sending data:', JSON.stringify(data));

    const { isConfirmed } = await Swal.fire({
        title: 'Are you sure?',
        text: 'Do you really want to edit this wallet?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#a6a6a6',
        confirmButtonText: 'Yes, edit it!',
        cancelButtonText: 'No, cancel!'
    });

    if (isConfirmed) {
        try {
            // Logic for determining if the balance has changed
            if (balance != oldBalance) {
                const transactionData = {
                    WalletId: id,
                    Amount: Math.abs(balance - oldBalance),
                    CategoryId: balance < oldBalance ? 2 : 1, // Category 2 for Expense, 1 for Income
                    TransactionType: balance < oldBalance ? 'Expense' : 'Income',
                    Note: balance < oldBalance ? 'Adjust Expense Wallet' : 'Adjust Income Wallet',
                    TransactionDate: new Date().toISOString(),
                    TransactionSort: 'Edit'

                };

                // Create the transaction if the balance has changed
                const transactionResponse = await fetch('/Transaction/Create', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': token
                    },
                    body: JSON.stringify(transactionData)
                });

                const transactionResult = await transactionResponse.json();
                if (!transactionResult.success) {
                    throw new Error(transactionResult.message || 'Transaction failed');
                }


                // Update wallet balance after the transaction is created
                const updateResponse = await fetch(`/Wallet/Edit/${id}`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': token
                    },
                    body: JSON.stringify(data)
                });

                const updateResult = await updateResponse.json();
                if (updateResult.success) {
                    closeModal(editAccountModal);
                    Swal.fire({
                        title: 'Success',
                        text: 'Wallet edited successfully!',
                        icon: 'success',
                        confirmButtonColor: '#3B82F6',
                        customClass: { popup: 'swal2-front' }
                    }).then(() => {
                        window.location.reload();
                    });
                } else {
                    Swal.fire({
                        title: 'Error',
                        text: updateResult.message || 'An error occurred.',
                        icon: 'error',
                        customClass: { popup: 'swal2-front' }
                    });
                }
            } else {
                // Update wallet balance after the transaction is created
                const updateResponse = await fetch(`/Wallet/Edit/${id}`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': token
                    },
                    body: JSON.stringify(data)
                });
                const updateResult = await updateResponse.json();
                if (updateResult.success) {
                    closeModal(editAccountModal);
                    Swal.fire({
                        title: 'Success',
                        text: 'Wallet edited successfully!',
                        icon: 'success',
                        confirmButtonColor: '#3B82F6',
                        customClass: { popup: 'swal2-front' }
                    }).then(() => {
                        window.location.reload();
                    });
                } else {
                    Swal.fire({
                        title: 'Error',
                        text: updateResult.message || 'An error occurred.',
                        icon: 'error',
                        customClass: { popup: 'swal2-front' }
                    });
                }
            }
        } catch (error) {
            Swal.fire({
                title: 'Error',
                text: error.message || 'Catch',
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
                    window.location.reload();
                });
            } else {
                Swal.fire({
                    title: 'Error',
                    text: 'Sorry, this wallet is being used in transaction, so you cannot delete this right now.',
                    icon: 'error',
                    customClass: { popup: 'swal2-front' }
                });
            }
        } catch (error) {
            Swal.fire({
                title: 'Error',
                text: 'Sorry, this wallet is being used in transaction, so you cannot delete this right now.',
                icon: 'error',
                customClass: { popup: 'swal2-front' }
            });
        }
    }

});

// Load accounts on page load
document.addEventListener('DOMContentLoaded', () => {
    loadDebitForm(); // Load the form fields initially
});

