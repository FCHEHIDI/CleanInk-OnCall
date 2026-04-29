/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './src/**/*.{html,ts}',
  ],
  theme: {
    extend: {
      colors: {
        primary:    '#1E5AA8',
        'primary-dk': '#0A1F44',
        ledger:     '#D9D9D9',
        accent:     '#3B82F6',
      },
      fontFamily: {
        sans: ['Inter', 'Roboto', 'sans-serif'],
      },
      borderRadius: {
        DEFAULT: '8px',
        lg: '12px',
        sm: '4px',
      },
    },
  },
  plugins: [],
  // Évite les conflits avec Angular Material
  corePlugins: {
    preflight: false,
  },
};
