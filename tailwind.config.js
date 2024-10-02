/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        './ASI.Basecode.WebApp/Views/**/*.cshtml', // Razor views
        './ASI.Basecode.WebApp/Pages/**/*.cshtml', // Razor Pages (if applicable)
        './ASI.Basecode.WebApp/wwwroot/**/*.html', // Any static HTML files in wwwroot
    ],
    darkMode: 'class',
    safelist: [
        // Ensure these dark mode classes are included in the final build
        'dark:bg-gray-900',
        'dark:bg-gray-800',
        'dark:bg-gray-700',
        'dark:text-gray-100',
        'dark:text-gray-300',
        'dark:border-gray-500',
        'dark:border-gray-600',
        'dark:hover:text-gray-500',
        'dark:hover:bg-gray-700',
        'dark:bg-opacity-75',
    ],
    theme: {
        extend: {
            colors: {
                primary: {
                    "50": "#eff6ff",
                    "100": "#dbeafe",
                    "200": "#bfdbfe",
                    "300": "#93c5fd",
                    "400": "#60a5fa",
                    "500": "#3b82f6",
                    "600": "#2563eb",
                    "700": "#1d4ed8",
                    "800": "#1e40af",
                    "900": "#1e3a8a",
                    "950": "#172554"
                },
            },
        },
        fontFamily: {
            'body': [
                'Inter',
                'ui-sans-serif',
                'system-ui',
                '-apple-system',
                'Segoe UI',
                'Roboto',
                'Helvetica Neue',
                'Arial',
                'Noto Sans',
                'sans-serif',
                'Apple Color Emoji',
                'Segoe UI Emoji',
                'Segoe UI Symbol',
                'Noto Color Emoji'
            ],
            'sans': [
                'Inter',
                'ui-sans-serif',
                'system-ui',
                '-apple-system',
                'Segoe UI',
                'Roboto',
                'Helvetica Neue',
                'Arial',
                'Noto Sans',
                'sans-serif',
                'Apple Color Emoji',
                'Segoe UI Emoji',
                'Segoe UI Symbol',
                'Noto Color Emoji'
            ]
        }
    },
    plugins: [],
}
