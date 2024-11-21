


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


    let trendsChart; // Global reference to the chart instance

    const renderTrendsChart = (labels, datasets) => {
        const canvas = document.getElementById('trendsChart');
        const ctx = canvas.getContext('2d');

        // If the chart already exists, destroy it before creating a new one
        if (trendsChart) {
            trendsChart.destroy();
        }

        // Create a new chart
        trendsChart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels, // Weekly or Monthly labels
                datasets: datasets // Dynamic datasets
            },
            options: {
                responsive: true,
                scales: {
                    y: {
                        beginAtZero: true,
                        grid: {
                            display: false // Disable horizontal grid lines
                        }
                    }
                }
            }
        });
    };

    const changeTrendView = (view) => {

        const expenseUrl = '/Transaction/GetMonthlyExpense';
        const incomeUrl = '/Transaction/GetMonthlyIncome';
        const url = view === 'weekly'
            ? '/Transaction/GetWeeklyTrends'
            : '/Transaction/GetMonthlyTrends';

        fetch(url)
            .then(response => response.json())
            .then(data => {
                if (view === 'weekly') {
                    const weeklyDays = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
                    const categoryData = {};

                    weeklyDays.forEach(day => {
                        data.filter(entry => entry.day === day).forEach(entry => {
                            if (!categoryData[entry.categoryName]) {
                                categoryData[entry.categoryName] = Array(7).fill(0);
                            }
                            const dayIndex = weeklyDays.indexOf(day);
                            if (dayIndex !== -1) {
                                categoryData[entry.categoryName][dayIndex] = entry.totalAmount;
                            }
                        });
                    });

                    // Generate datasets for the chart
                    const datasets = Object.entries(categoryData).map(([categoryName, data]) => ({
                        label: categoryName,
                        data: data,
                        borderColor: `rgba(${Math.random() * 255}, ${Math.random() * 255}, ${Math.random() * 255}, 1)`,
                        backgroundColor: 'rgba(0,0,0,0)',
                        fill: true,
                        tension: 0.5,
                    }));

                    renderTrendsChart(weeklyDays, datasets); // Render with correct labels
                } else {
                    // Monthly labels (Jan-Dec)
                    const allMonths = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
                    const categoryData = {};

                    allMonths.forEach(month => {
                        data.filter(entry => new Date(entry.month).toLocaleString('default', { month: 'short' }) === month)
                            .forEach(entry => {
                                if (!categoryData[entry.categoryName]) {
                                    categoryData[entry.categoryName] = Array(12).fill(0);
                                }
                                const monthIndex = allMonths.indexOf(month);
                                if (monthIndex !== -1) {
                                    categoryData[entry.categoryName][monthIndex] = entry.totalAmount;
                                }
                            });
                    });

                    const datasets = Object.entries(categoryData).map(([categoryName, data]) => ({
                        label: categoryName,
                        data: data,
                        borderColor: `rgba(${Math.random() * 255}, ${Math.random() * 255}, ${Math.random() * 255}, 1)`,
                        backgroundColor: 'rgba(0,0,0,0)',
                        fill: true,
                        tension: 0.5,
                    }));

                    renderTrendsChart(allMonths, datasets);
                }
            })
            .catch(error => console.error(`Error fetching ${view} trends data:`, error));
    };

    // Attach to global scope
    window.changeTrendView = changeTrendView;


    // Initial load: Monthly trends by default
    changeTrendView('monthly');

    const renderMonthlyExpenseChart = () => {
        const expenseUrl = '/Transaction/GetMonthlyExpense';

        fetch(expenseUrl)
            .then(response => response.json())
            .then(data => {
                const allMonths = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
                const formattedExpenses = Array(12).fill(0);

                data.monthlyData.forEach(entry => {
                    const monthIndex = new Date(`${entry.month}-01`).getMonth();
                    formattedExpenses[monthIndex] = entry.totalExpense;
                });

                const ctx = document.getElementById('monthlyExpenseChart').getContext('2d');
                new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: allMonths,
                        datasets: [{
                            label: 'Monthly Expense',
                            data: formattedExpenses,
                            backgroundColor: 'rgba(255, 99, 132, 0.2)',
                            borderColor: 'rgba(255, 99, 132, 1)',
                            borderWidth: 1,
                        }],
                    },
                    options: {
                        responsive: true,
                        scales: {
                            y: {
                                beginAtZero: true,
                            },
                        },
                    },
                });
            })
            .catch(error => console.error('Error fetching monthly expenses:', error));
    };

    const renderMonthlyIncomeChart = () => {
        const incomeUrl = '/Transaction/GetMonthlyIncome';

        fetch(incomeUrl)
            .then(response => response.json())
            .then(data => {
                const allMonths = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
                const formattedIncome = Array(12).fill(0);

                data.monthlyData.forEach(entry => {
                    const monthIndex = new Date(`${entry.month}-01`).getMonth();
                    formattedIncome[monthIndex] = entry.totalIncome;
                });

                const ctx = document.getElementById('monthlyIncomeChart').getContext('2d');
                new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: allMonths,
                        datasets: [{
                            label: 'Monthly Income',
                            data: formattedIncome,
                            backgroundColor: 'rgba(75, 192, 192, 0.2)',
                            borderColor: 'rgba(75, 192, 192, 1)',
                            borderWidth: 1,
                        }],
                    },
                    options: {
                        responsive: true,
                        scales: {
                            y: {
                                beginAtZero: true,
                            },
                        },
                    },
                });
            })
            .catch(error => console.error('Error fetching monthly income:', error));
    };

    // Render both charts on page load
    renderMonthlyExpenseChart();
    renderMonthlyIncomeChart();

}); 
function toggleDropdown() {
    const dropdown = document.getElementById('trendsDropdownMenu');
    const isHidden = dropdown.classList.contains('hidden');

    if (isHidden) {
        dropdown.classList.remove('hidden', 'opacity-0');
        dropdown.classList.add('block', 'opacity-100');
    } else {
        dropdown.classList.remove('block', 'opacity-100');
        dropdown.classList.add('hidden', 'opacity-0');
    }
}