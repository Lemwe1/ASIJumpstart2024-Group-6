// Modal Elements
const addAccountModal = document.getElementById('addAccountModal');
const addAccountButton = document.getElementById('addAccountButton');
const modalContent = document.getElementById('modalContent');

// Form field elements
const addFormFields = document.getElementById('addFormFields');
const addDebitType = document.getElementById('addDebitType');
const addBorrowedType = document.getElementById('addBorrowedType');

// Function to open modal
function openModal(modal) {
    modal.classList.remove('hidden');
}

// Function to close modal
function closeModal(modal) {
    modal.classList.add('hidden');
}

// Open the modal when the Add Account button is clicked
addAccountButton.addEventListener('click', () => {
    openModal(addAccountModal);
});

// Close the modal if the background (not the modal content) is clicked
addAccountModal.addEventListener('click', (e) => {
    if (e.target === addAccountModal) { // Only close if clicked outside modal content
        closeModal(addAccountModal);
    }
});

// Load fields for Debit type
addDebitType.addEventListener('click', () => {
    addFormFields.innerHTML = `
        <div class="mb-4">
            <label class="block">Name</label>
            <input type="text" class="border p-2 w-full" required />
        </div>
        <div class="mb-4">
            <label class="block">Balance</label>
            <input type="number" class="border p-2 w-full" required />
        </div>
        <div class="mb-4">
            <label class="block">Icon</label>
            <select class="border p-2 w-full" required>
                <option value="fas fa-wallet">Wallet</option>
                <option value="fas fa-piggy-bank">Piggy Bank</option>
                <option value="fas fa-coins">Coins</option>
                <option value="fas fa-credit-card">Credit Card</option>
                <option value="fas fa-building">Building</option>
            </select>
        </div>
        <div class="mb-4">
            <label class="block">Color</label>
            <input type="color" class="border p-2 w-full" required />
        </div>
    `;
});

// Load fields for Borrowed type
addBorrowedType.addEventListener('click', () => {
    addFormFields.innerHTML = `
        <div class="mb-4">
            <label class="block">Name</label>
            <input type="text" class="border p-2 w-full" required />
        </div>
        <div class="mb-4">
            <label class="block">Amount Borrowed</label>
            <input type="number" class="border p-2 w-full" required />
        </div>
        <div class="mb-4">
            <label class="block">Destination Account</label>
            <select class="border p-2 w-full" required>
                <option value="1">Account 1</option>
                <option value="2">Account 2</option>
            </select>
        </div>
        <div class="mb-4">
            <label class="block">Icon</label>
            <select class="border p-2 w-full" required>
                <option value="fas fa-wallet">Wallet</option>
                <option value="fas fa-piggy-bank">Piggy Bank</option>
                <option value="fas fa-coins">Coins</option>
                <option value="fas fa-credit-card">Credit Card</option>
                <option value="fas fa-building">Building</option>
            </select>
        </div>
        <div class="mb-4">
            <label class="block">Color</label>
            <input type="color" class="border p-2 w-full" required />
        </div>
        <div class="mb-4">
            <label class="block">Happening Date</label>
            <input type="date" class="border p-2 w-full" required />
        </div>
        <div class="mb-4">
            <label class="block">Due Date</label>
            <input type="date" class="border p-2 w-full" required />
        </div>
    `;
<<<<<<< HEAD
});
=======
});
>>>>>>> 51c208272ba491f3a0cb176cd8638b9aa32ac5d7
