import DefaultTheme from 'vitepress/theme';
import './custom.css';
import Redoc from '../../components/Redoc.vue';

export default {
  ...DefaultTheme,
  // Custom theme integration point
  enhanceApp({ app, router, siteData }) {
    app.component('Redoc', Redoc);
  },
};
