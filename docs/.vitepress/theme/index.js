import DefaultTheme from 'vitepress/theme';
import './custom.css';

export default {
  ...DefaultTheme,
  // Custom theme integration point
  enhanceApp({ app, router, siteData }) {
    // Custom logic if needed
  },
};
