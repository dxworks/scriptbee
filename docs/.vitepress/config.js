import { defineConfig } from 'vitepress';

export default defineConfig({
  title: 'ScriptBee Docs',
  description: 'Documentation for ScriptBee',
  base: '/scriptbee/',
  head: [['link', { rel: 'icon', href: '/scriptbee/assets/logo_bee.ico' }]],
  themeConfig: {
    logo: '/assets/logo.svg',

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
          {
            text: 'Installation',
            link: '/installation/installation',
            items: [
              { text: 'Docker Compose', link: '/installation/docker_installation' },
              { text: 'Kubernetes', link: '/installation/kubernetes_installation' },
              { text: 'VS Code', link: '/installation/vs_code_extension' },
            ],
          },
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
          { text: 'Overview', link: '/architecture/overview' },
          { text: 'Diagram', link: '/architecture/diagram' },
          {
            text: 'Configuration',
            items: [
              {
                text: 'Gateway Configuration',
                link: '/architecture/configuration/gateway_configuration',
              },
              {
                text: 'Analysis Configuration',
                link: '/architecture/configuration/analysis_configuration',
              },
            ],
          },
          {
            text: 'REST API',
            link: '/architecture/rest_api',
            items: [
              { text: 'Gateway REST API', link: '/architecture/gateway_rest_api' },
              { text: 'Analysis REST API', link: '/architecture/analysis_rest_api' },
            ],
          },
          { text: 'RBAC', link: '/architecture/rbac' },
          {
            text: 'Internals',
            link: '/architecture/internals',
            items: [
              {
                text: 'Generate Model Classes',
                link: '/architecture/internals/generate_classes_streaming_protocol',
              },
            ],
          },
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
