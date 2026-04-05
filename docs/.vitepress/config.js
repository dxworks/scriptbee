import { defineConfig } from 'vitepress';

export default defineConfig({
  title: 'ScriptBee Docs',
  description: 'Documentation for ScriptBee',
  themeConfig: {
    logo: '/assets/logo.png',
    nav: [
      { text: 'Home', link: '/' },
      { text: 'Projects', link: '/projects/creation' },
      { text: 'Plugins', link: '/plugins/installation' },
      { text: 'Scripts', link: '/scripts/setup' },
      { text: 'Results', link: '/results/view' },
      { text: 'Architecture', link: '/architecture/overview' },
    ],
    sidebar: [
      {
        text: 'Home',
        items: [
          { text: 'Introduction', link: '/' },
          { text: 'Installation', link: '/home/installation' },
          { text: 'Run', link: '/home/run' },
        ],
      },
      {
        text: 'Projects',
        items: [
          { text: 'Creation', link: '/projects/creation' },
          { text: 'Context', link: '/projects/context' },
          { text: 'Deletion', link: '/projects/deletion' },
        ],
      },
      {
        text: 'Plugins',
        items: [
          { text: 'Installation', link: '/plugins/installation' },
          { text: 'Manifest', link: '/plugins/manifest' },
          {
            text: 'Plugin Api',
            items: [
              { text: 'Overview', link: '/plugins/plugin_api' },
              { text: 'Loader', link: '/plugins/loader' },
              { text: 'Linker', link: '/plugins/linker' },
              { text: 'Script Generator', link: '/plugins/script_generator' },
              { text: 'Script Runner', link: '/plugins/script_runner' },
              {
                text: 'Helper Functions',
                items: [
                  { text: 'Api', link: '/plugins/helper_functions' },
                  { text: 'Console', link: '/plugins/helper_functions/console_helper_functions' },
                  { text: 'File', link: '/plugins/helper_functions/file_helper_functions' },
                  { text: 'Json', link: '/plugins/helper_functions/json_helper_functions' },
                  { text: 'Csv', link: '/plugins/helper_functions/csv_helper_functions' },
                  { text: 'Context', link: '/plugins/helper_functions/context_helper_functions' },
                ],
              },
              { text: 'Bundle', link: '/plugins/bundle' },
            ],
          },
        ],
      },
      {
        text: 'Scripts',
        items: [
          { text: 'Setup', link: '/scripts/setup' },
          { text: 'Writing Your First C# Script', link: '/scripts/first_csharp_script' },
          { text: 'Writing Your First Python Script', link: '/scripts/first_python_script' },
          {
            text: 'Writing Your First Javascript Script',
            link: '/scripts/first_javascript_script',
          },
        ],
      },
      {
        text: 'Results',
        items: [{ text: 'View Results', link: '/results/view' }],
      },
      {
        text: 'Architecture',
        items: [
          { text: 'Diagram', link: '/architecture/diagram' },
          { text: 'Overview', link: '/architecture/overview' },
          { text: 'Features', link: '/architecture/features' },
          { text: 'RBAC', link: '/architecture/rbac' },
        ],
      },
    ],
    socialLinks: [{ icon: 'github', link: 'https://github.com/dxworks/scriptbee' }],
    footer: {
      message: 'Released under the MIT License.',
      copyright: 'Copyright © 2024-present',
    },
    search: {
      provider: 'local',
    },
  },
});
