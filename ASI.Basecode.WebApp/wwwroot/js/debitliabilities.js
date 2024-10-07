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

// Function to update the net worth section
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

    // Render debit accounts
    debitAccounts.forEach(account => {
        document.getElementById('debitAccounts').innerHTML += `
            <div class="accountCard" style="background-color: ${account.DeLiColor}">
                <i class="${account.DeLiIcon} text-lg"></i>
                <h3 class="font-bold text-lg">${account.DeLiName}</h3>
                <p>₱${account.DeLiBalance.toFixed(2)}</p>
            </div>
        `;
    });

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
            alert(result.message);

            // Close the modal
            closeModal(addAccountModal);

            // Refresh the account lists
            await loadAccounts(); // Fetch updated accounts from the server and render them
            // Automatically refresh the page
            location.reload();

        } else {
            console.error(result.errors);
            alert(result.message);
        }
    } catch (error) {
        console.error('Error saving account:', error);
        alert('Error saving account');
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
