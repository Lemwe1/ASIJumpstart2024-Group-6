// Modal Elements
const addAccountModal = document.getElementById('addAccountModal');
const addAccountButton = document.getElementById('addAccountButton');
const modalContent = document.getElementById('modalContent');

// Form field elements
const addFormFields = document.getElementById('addFormFields');

// Arrays to store accounts (can be removed if stored in the DB)
let debitAccounts = [];

function updateNetWorth() {
    // Calculate the total debit and total liabilities
    let totalDebit = debitAccounts.reduce((acc, account) => acc + account.WalletBalance, 0);

    // Update the net worth section in the HTML
    document.getElementById('balanceSection').innerHTML = `
        <h2 class="text-2xl font-bold">Net Worth: ₱${totalDebit.toFixed(2)}</h2>
    `;
}

// Function to open modal
function openModal(modal) {
    if (modal) {
        modal.classList.remove('hidden');
    } else {
        console.error('Modal element not found.');
    }
}

// Function to close modal
function closeModal(modal) {
    if (modal) {
        modal.classList.add('hidden');
    } else {
        console.error('Modal element not found.');
    }
}

// Open the modal when the Add Account button is clicked
if (addAccountButton) {
    addAccountButton.addEventListener('click', () => {
        openModal(addAccountModal);
    });
} else {
    console.error('Add Account Button not found.');
}

// Close the modal if the background (not the modal content) is clicked
if (addAccountModal) {
    addAccountModal.addEventListener('click', (e) => {
        if (e.target === addAccountModal) { // Only close if clicked outside modal content
            closeModal(addAccountModal);
        }
    });
} else {
    console.error('Add Account Modal not found.');
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
                <input type="number" id="accountBalance" class="border p-2 w-full" required />
            </div>
            <div class="mb-6">
                <label for="createIcon" class="block text-sm font-medium text-gray-700">Icon</label>
                <select name="Icon" id="createIcon" class="mt-1 block w-full px-4 py-2 border rounded-md bg-white text-gray-900 focus:ring-blue-500 focus:border-blue-500">
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
            await loadAccounts(); // Fetch updated accounts from the server and render them
            // Automatically refresh the page
            location.reload();
        } else {
            console.error(result.errors);
        }
    } catch (error) {
        console.error('Error saving account:', error);
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
        console.error('Error fetching accounts:', error);
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

    openModal(editAccountModal);
}

// Close the edit modal if the background (not the modal content) is clicked
if (editAccountModal) {
    editAccountModal.addEventListener('click', (e) => {
        if (e.target === editAccountModal) { // Only close if clicked outside modal content
            closeModal(editAccountModal);
        }
    });
} else {
    console.error('Edit Account Modal not found.');
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
            await loadAccounts(); // Fetch updated accounts from the server and render them
            // Automatically refresh the page
            location.reload();
        } else {
            console.error(result.errors);
        }
    } catch (error) {
        console.error('Error updating account:', error);
    }
});

// Handle account deletion
document.getElementById('deleteAccountButton').addEventListener('click', async () => {
    const id = document.getElementById('editAccountId').value;

    if (!id) {
        console.error('No account ID specified for deletion.');
        return;
    }

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
            await loadAccounts(); // Fetch updated accounts from the server and render them
            // Automatically refresh the page
            location.reload();
        } else {
            console.error(result.errors);
        }
    } catch (error) {
        console.error('Error deleting account:', error);
    }
});

// Load accounts on page load
document.addEventListener('DOMContentLoaded', () => {
    loadAccounts();
    loadDebitForm(); // Load the form fields initially
});
