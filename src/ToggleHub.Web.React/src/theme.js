import { createTheme } from '@mui/material/styles';

const theme = createTheme({
  palette: {
    mode: 'dark', // Force dark mode always
    primary: {
      main: '#646cff',
    },
    secondary: {
      main: '#535bf2',
    },
    background: {
      default: '#242424',
      paper: '#1a1a1a',
    },
    text: {
      primary: 'rgba(255, 255, 255, 0.87)',
      secondary: 'rgba(255, 255, 255, 0.6)',
    },
  },
  components: {
    MuiCssBaseline: {
      styleOverrides: {
        // Force dark theme at component level
        body: {
          backgroundColor: '#242424',
          color: 'rgba(255, 255, 255, 0.87)',
        },
        '*': {
          // Prevent any light theme from taking over
          colorScheme: 'dark !important',
        }
      },
    },
    MuiButton: {
      styleOverrides: {
        root: {
          borderRadius: 0,
        },
      },
    },
  },
});

export default theme;
