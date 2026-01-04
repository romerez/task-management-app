import React from 'react';
import { Provider } from 'react-redux';
import { ThemeProvider, createTheme } from '@mui/material';
import { store } from './store/store';
import { AppContent } from './components/AppContent';

const theme = createTheme({
    palette: {
        primary: {
            main: '#1976d2',
        },
        secondary: {
            main: '#dc004e',
        },
        background: {
            default: '#f5f5f5',
        },
    },
    typography: {
        h4: {
            fontWeight: 600,
        },
    },
});

export const App: React.FC = () => (
    <ThemeProvider theme={theme}>
        <Provider store={store}>
            <AppContent />
        </Provider>
    </ThemeProvider>
);