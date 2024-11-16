document.addEventListener('DOMContentLoaded', () => {
    const fadeOutDialog = (dialog, duration) => {
        setTimeout(() => {
            dialog.classList.remove('opacity-100'); // Fade out
            dialog.classList.add('opacity-0'); // Make it invisible
        }, duration);
    };

    const initializeDialog = () => {
        const dialog = document.getElementById('dialog-message');

        if (dialog) {
            dialog.classList.remove('opacity-0');
            dialog.classList.add('opacity-100'); // Fade in

            setTimeout(() => {
                fadeOutDialog(dialog, 3000);
            }, 3000); // visible for 3 seconds
        }
    };

    initializeDialog();

    // Check if 'dark' theme is saved in localStorage or applied on body
    const savedTheme = localStorage.getItem('theme') || 'light';
    if (savedTheme === 'dark' || document.body.classList.contains('dark')) {
        document.body.classList.add('dark');
        localStorage.setItem('theme', 'dark');
    } else {
        document.body.classList.remove('dark');
        localStorage.setItem('theme', 'light');
    }

    // Render Trends Chart (Line chart with curvy edges)
    fetch('/Transaction/GetMonthlyTrends')
        .then(response => response.json())
        .then(data => {
            console.log('Monthly Trends Data:', data); // Confirm data structure
            const canvas = document.getElementById('trendsChart');
            if (canvas) {
                const ctx = canvas.getContext('2d');
                new Chart(ctx, {
                    type: 'line',
                    data: {
                        labels: data.map(x => x.month), // Month labels
                        datasets: [
                            {
                                label: 'Total Expenses',
                                data: data.map(x => x.totalExpense), // TotalExpense data
                                borderColor: 'rgba(255, 99, 132, 1)',
                                backgroundColor: 'rgba(255, 99, 132, 0.2)',
                                fill: true,
                                tension: 0.4 // Curvy edges
                            },
                            {
                                label: 'Total Income',
                                data: data.map(x => x.totalIncome), // TotalIncome data
                                borderColor: 'rgba(54, 162, 235, 1)',
                                backgroundColor: 'rgba(54, 162, 235, 0.2)',
                                fill: true,
                                tension: 0.4 // Curvy edges
                            }
                        ]
                    },
                    options: {
                        responsive: true,
                        scales: {
                            y: {
                                beginAtZero: true
                            }
                        }
                    }
                });
            } else {
                console.error('Canvas element for trendsChart not found');
            }
        })
        .catch(error => {
            console.error('Error fetching Monthly Trends data:', error);
        });

    // Render Monthly Expense Chart (Line chart with curvy edges)
    fetch('/Transaction/GetMonthlyExpense')
        .then(response => response.json())
        .then(data => {
            const canvas = document.getElementById('monthlyExpenseChart');
            if (canvas) {
                const ctx = canvas.getContext('2d');
                new Chart(ctx, {
                    type: 'line', // Change to 'line'
                    data: {
                        labels: data.map(x => x.month), // Month labels
                        datasets: [{
                            label: 'Monthly Expenses',
                            data: data.map(x => x.totalExpense), // TotalExpense data
                            borderColor: 'rgba(255, 99, 132, 1)',
                            backgroundColor: 'rgba(255, 99, 132, 0.2)',
                            fill: true,
                            tension: 0.4 // Curvy edges
                        }]
                    },
                    options: {
                        responsive: true,
                        scales: {
                            y: {
                                beginAtZero: true
                            }
                        }
                    }
                });
            } else {
                console.error('Canvas element for monthlyExpenseChart not found');
            }
        });

    // Render Monthly Income Chart (Line chart with curvy edges)
    fetch('/Transaction/GetMonthlyIncome')
        .then(response => response.json())
        .then(data => {
            const canvas = document.getElementById('monthlyIncomeChart');
            if (canvas) {
                const ctx = canvas.getContext('2d');
                new Chart(ctx, {
                    type: 'line', // Change to 'line'
                    data: {
                        labels: data.map(x => x.month), // Month labels
                        datasets: [{
                            label: 'Monthly Income',
                            data: data.map(x => x.totalIncome), // TotalIncome data
                            borderColor: 'rgba(54, 162, 235, 1)',
                            backgroundColor: 'rgba(54, 162, 235, 0.2)',
                            fill: true,
                            tension: 0.4 // Curvy edges
                        }]
                    },
                    options: {
                        responsive: true,
                        scales: {
                            y: {
                                beginAtZero: true
                            }
                        }
                    }
                });
            } else {
                console.error('Canvas element for monthlyIncomeChart not found');
            }
        });
});