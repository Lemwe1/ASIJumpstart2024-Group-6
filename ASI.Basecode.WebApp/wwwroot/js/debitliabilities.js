// Modal Elements
const addAccountModal = document.getElementById('addAccountModal');
const addAccountButton = document.getElementById('addAccountButton');
const modalContent = document.getElementById('modalContent');

// Form field elements
const addFormFields = document.getElementById('addFormFields');
const addDebitType = document.getElementById('addDebitType');
const addBorrowedType = document.getElementById('addBorrowedType');
const selectedAccountType = document.getElementById('selectedAccountType'); // Hidden input to store the selected account type

// Sections Titles
const debitSectionTitle = document.getElementById('debitSectionTitle');
const borrowedSectionTitle = document.getElementById('borrowedSectionTitle');

// Arrays to store accounts (can be removed if stored in the DB)
let debitAccounts = [];
let borrowedAccounts = [];

// Flag to track if changes were made in the modal
let isModalDirty = false;

// Function to update the net worth section
function updateNetWorth() {
    // Calculate the total debit and total liabilities
    let totalDebit = debitAccounts.reduce((acc, account) => acc + account.DeLiBalance, 0);
    let totalLiabilities = borrowedAccounts.reduce((acc, account) => acc + account.DeLiBalance, 0);
    let netWorth = totalDebit - totalLiabilities;

    // Update the net worth section in the HTML
    document.getElementById('netWorthSection').innerHTML = `
        <h2 class="text-2xl font-bold">Net Worth: ₱${netWorth.toFixed(2)}</h2>
        <div>
            <p>Total Debit: ₱${totalDebit.toFixed(2)}</p>
            <p>Total Liabilities: ₱${totalLiabilities.toFixed(2)}</p>
        </div>
    `;
}

// Function to render debit and borrowed accounts
function renderAccounts() {
    // Clear existing accounts
    document.getElementById('debitAccounts').innerHTML = '';
    document.getElementById('borrowedAccounts').innerHTML = '';

    // Render borrowed accounts
    borrowedAccounts.forEach(account => {
        document.getElementById('borrowedAccounts').innerHTML += `
            <div class="accountCard" style="background-color: ${account.DeLiColor}">
                <i class="${account.DeLiIcon} text-lg"></i>
                <h3 class="font-bold text-lg">${account.DeLiName}</h3>
                <p>₱${account.DeLiBalance.toFixed(2)}</p>
                <p>Happening Date: ${account.DeLiHapp}</p>
                <p>Due Date: ${account.DeLiDue}</p>
            </div>
        `;
    });
    // Render debit accounts
    debitAccounts.forEach(account => {
        document.getElementById('debitAccounts').innerHTML += `
            <div class="accountCard" style="background-color: ${account.DeLiColor}">
                <i class="${account.DeLiIcon} text-lg"></i>
                <h3 class="font-bold text-lg">${account.DeLiName}</h3>
                <p>₱${account.DeLiBalance.toFixed(2)}</p>
                <button class="editButton" onclick="openEditModal(${JSON.stringify(account)})">Edit</button>
            </div>
        `;
    });
}
if (editAccountModal) {
    editAccountModal.addEventListener('click', (e) => {
        if (e.target === editAccountModal) { // Only close if clicked outside modal content
            closeModal(editAccountModal);
        }
    });
} else {
    console.error('Edit Account Modal not found.');

    // Update the net worth
    updateNetWorth();

    // Toggle visibility of the "Debit" and "Borrowed" headers
    toggleSectionTitles();
}


// Function to toggle visibility of the Debit and Borrowed headers
function toggleSectionTitles() {
    // Show or hide the "Debit" header
    debitSectionTitle.classList.toggle('hidden', debitAccounts.length === 0);
    borrowedSectionTitle.classList.toggle('hidden', borrowedAccounts.length === 0);
}


// Function to open modal
function openModal(modal) {
    if (modal) {
        modal.classList.remove('hidden');
    } else {
        console.error('Modal element not found.');
    }
}

// Function to close modal with confirmation
function closeModal(modal) {
    if (isModalDirty) {
        if (confirm("You have unsaved changes. Do you want to discard them?")) {
            modal.classList.add('hidden');
            isModalDirty = false; // Reset dirty flag
        }
    } else {
        modal.classList.add('hidden');
    }
}

// Open the modal when the Add Account button is clicked
if (addAccountButton) {
    addAccountButton.addEventListener('click', () => {
        // Default to "Debit" with blue background
        setTypeSelection('debit');  // Set default value to 'debit'
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

// Function to handle type selection (Debit or Borrowed)
function setTypeSelection(type) {
    if (selectedAccountType) {
        selectedAccountType.value = type;  // Set the hidden input value to 'debit' or 'borrowed'
    } else {
        console.error('Selected Account Type element not found.');
        return;
    }

    if (type === 'debit') {
        if (addDebitType) {
            addDebitType.classList.add('bg-blue-500', 'text-white');
        }
        if (addBorrowedType) {
            addBorrowedType.classList.remove('bg-blue-500', 'text-white');
            addBorrowedType.classList.add('bg-gray-100', 'text-black');
        }

        // Load Debit form fields
        loadDebitForm();
    } else {
        if (addBorrowedType) {
            addBorrowedType.classList.add('bg-blue-500', 'text-white');
        }
        if (addDebitType) {
            addDebitType.classList.remove('bg-blue-500', 'text-white');
            addDebitType.classList.add('bg-gray-100', 'text-black');
        }

        // Load Borrowed form fields
        loadBorrowedForm();
    }
}

// Handle form field input to set dirty flag
document.getElementById('addAccountForm').addEventListener('input', () => {
    isModalDirty = true; // Set dirty flag when form input is changed
});

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

// Function to load Borrowed form fields with Happening Date and Due Date
function loadBorrowedForm() {
    if (addFormFields) {
        addFormFields.innerHTML = `
            <div class="mb-4">
                <label class="block">Name</label>
                <input type="text" id="accountName" class="border p-2 w-full" required />
            </div>
            <div class="mb-4">
                <label class="block">Amount Borrowed</label>
                <input type="number" id="accountAmount" class="border p-2 w-full" required />
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
                <label class="block">Happening Date</label>
                <input type="date" id="happeningDate" class="border p-2 w-full" required />
            </div>
            <div class="mb-4">
                <label class="block">Due Date</label>
                <input type="date" id="dueDate" class="border p-2 w-full" required />
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

// Event listeners for handling type selection
if (addDebitType) {
    addDebitType.addEventListener('click', () => {
        setTypeSelection('debit');
    });
} else {
    console.error('Add Debit Type button not found.');
}

if (addBorrowedType) {
    addBorrowedType.addEventListener('click', () => {
        setTypeSelection('borrowed');
    });
} else {
    console.error('Add Borrowed Type button not found.');
}

// Handle form submission
document.getElementById('addAccountForm').addEventListener('submit', async (e) => {
    e.preventDefault();

    const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
    const token = tokenElement ? tokenElement.value : null;

    const type = selectedAccountType.value;
    const name = document.getElementById('accountName').value;
    const color = document.getElementById('accountColor').value;
    const icon = document.getElementById('createIcon').value;

    let data = {
        DeLiType: type,
        DeLiIcon: icon,
        DeLiColor: color,
        DeLiBalance: 0,
        DeLiName: name
    };

    if (type === 'debit') {
        const balance = parseFloat(document.getElementById('accountBalance').value);
        data.DeLiBalance = balance;
    } else if (type === 'borrowed') {
        const amount = parseFloat(document.getElementById('accountAmount').value);
        const happeningDate = document.getElementById('happeningDate').value;
        const dueDate = document.getElementById('dueDate').value;

        data.DeLiBalance = amount;
        data.DeLiHapp = happeningDate ? new Date(happeningDate).toISOString() : null;
        data.DeLiDue = dueDate ? new Date(dueDate).toISOString() : null;
    }

    console.log('Sending data:', JSON.stringify(data));

    try {
        const response = await fetch('/DebitLiabilities/Create', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify(data)
        });

        const result = await response.json();
        if (result.success) {
            // alert(result.message);

            // Close the modal
            closeModal(addAccountModal);

            // Refresh the account lists
            await loadAccounts(); // Fetch updated accounts from the server and render them
            // Automatically refresh the page
            location.reload();

        } else {
            console.error(result.errors);
            //alert(result.message);
        }
    } catch (error) {
        console.error('Error saving account:', error);
        //alert('Error saving account');
    }
});

// Function to load accounts from the server and render them
async function loadAccounts() {
    try {
        const response = await fetch('/DebitLiabilities'); // Adjust URL if necessary
        const accounts = await response.json();

        // Separate accounts into debit and borrowed categories
        debitAccounts = accounts.filter(account => account.DeLiType === 'debit');
        borrowedAccounts = accounts.filter(account => account.DeLiType === 'borrowed');

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
    document.getElementById('editAccountId').value = account.DeLiId || ''; // Default to empty string if undefined
    document.getElementById('editAccountName').value = account.DeLiName || ''; // Default to empty string if undefined
    document.getElementById('editAccountBalance').value = account.DeLiBalance || ''; // Default to empty string if undefined
    document.getElementById('editAccountColor').value = account.DeLiColor || '#000000'; // Default color if undefined
    document.getElementById('editCreateIcon').value = account.DeLiIcon || ''; // Default to empty string if undefined

    // Show the appropriate fields based on the account type
    if (account.DeLiType === 'borrowed') {
        document.getElementById('editBorrowedFields').classList.remove('hidden');
        document.getElementById('editHappeningDate').value = account.DeLiHapp ? new Date(account.DeLiHapp).toISOString().substring(0, 10) : '';
        document.getElementById('editDueDate').value = account.DeLiDue ? new Date(account.DeLiDue).toISOString().substring(0, 10) : '';
        setTypeSelection('borrowed');
    } else {
        document.getElementById('editBorrowedFields').classList.add('hidden');
        setTypeSelection('debit');
    }

    openModal(editAccountModal);
}


    if (editAccountModal) {
        editAccountModal.addEventListener('click', (e) => {
            if (e.target === editAccountModal) { // Only close if clicked outside modal content
                closeModal(editAccountModal);
            }
        });
    } else {
        console.error('Edit Account Modal not found.');
}

document.getElementById('editAccountForm').addEventListener('submit', async (e) => {
    e.preventDefault();

    const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
    const token = tokenElement ? tokenElement.value : null;

    const id = document.getElementById('editAccountId').value;
    const name = document.getElementById('editAccountName').value;
    const color = document.getElementById('editAccountColor').value;
    const icon = document.getElementById('editCreateIcon').value;
    const type = selectedAccountType.value; // This should reflect the account type

    let data = {
        DeLiId: id,
        DeLiType: type,
        DeLiIcon: icon,
        DeLiColor: color,
        DeLiName: name,
        DeLiBalance: 0 // Set this dynamically for debit or borrowed
    };

    if (type === 'debit') {
        const balance = parseFloat(document.getElementById('editAccountBalance').value);
        data.DeLiBalance = balance;
    } else if (type === 'borrowed') {
        const amount = parseFloat(document.getElementById('accountAmount').value);
        const happeningDate = document.getElementById('editHappeningDate').value;
        const dueDate = document.getElementById('editDueDate').value;

        data.DeLiBalance = amount;
        data.DeLiHapp = happeningDate ? new Date(happeningDate).toISOString() : null;
        data.DeLiDue = dueDate ? new Date(dueDate).toISOString() : null;
    }

    console.log('Sending data:', JSON.stringify(data));

    try {
        const response = await fetch(`/DebitLiabilities/Edit/${id}`, {
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

document.getElementById('deleteAccountButton').addEventListener('click', async () => {
    const id = document.getElementById('editAccountId').value;

    if (confirm('Are you sure you want to delete this account?')) {
        // Retrieve CSRF token
        const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
        const token = tokenElement ? tokenElement.value : null;

        try {
            const response = await fetch(`/DebitLiabilities/Delete/${id}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                }
            });

            if (!response.ok) {
                console.error('Server responded with an error:', await response.text());
                return;
            }

            const result = await response.json();
            if (result.success) {
                closeModal(editAccountModal);
                await loadAccounts(); // Refresh account list
                // Automatically refresh the page
                location.reload();
            } else {
                console.error('Deletion failed:', result.message);
            }
        } catch (error) {
            console.error('Error deleting account:', error);
        }
    }
});


