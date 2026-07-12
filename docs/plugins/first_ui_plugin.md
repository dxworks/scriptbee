# Writing Your First UI Plugin

> [!IMPORTANT]
> **Angular Version Compatibility:** Before starting, ensure the Angular version of your plugin matches the Angular
> version used by the ScriptBee client. Check the core client's `package.json` for the exact version required.

UI plugins are standalone Angular applications loaded into ScriptBee at runtime via
[Angular Native Federation](https://www.angulararchitects.io/en/blog/combining-native-federation-and-module-federation/).
There are three types of UI plugin outlets you can implement:

- [**Top Navigation Bar**](#top-navigation-bar-outlet) — adds a link to the global navigation bar
- [**Side Panel**](#side-panel-outlet) — adds an entry in the side panel
- [**File Previewer**](#file-previewer-outlet) — renders file content in the output panel

## 📚 Prerequisites and Resources

To successfully develop a modern UI plugin, familiarize yourself with these concepts:

- **Angular Native Federation:** This is the underlying technology that allows micro-frontends to work together
  seamlessly.
  - [Combining Native Federation and Module Federation](https://www.angulararchitects.io/en/blog/combining-native-federation-and-module-federation/)
  - [Native Federation: Just Got Better Performance DX and Simplicity](https://www.angulararchitects.io/en/blog/native-federation-just-got-better-performance-dx-and-simplicity/)
  - [Micro-Frontends with Angular and Native Federation (Part 1)](https://blog.angular.dev/micro-frontends-with-angular-and-native-federation-7623cfc5f413)
  - [Micro-Frontends with Modern Angular (Standalone and Esbuild)](https://www.angulararchitects.io/en/blog/micro-frontends-with-modern-angular-part-1-standalone-and-esbuild)

---

An example plugin is available in
the [ScriptBee Default Plugin Bundle](https://github.com/dxworks/scriptbee/tree/master/Plugins/UI/default-scriptbee-charts)

## Project Setup

### 1. Create a new Angular application

```bash
npx -y @angular/cli@21 new my-scriptbee-plugin --standalone --routing --style=css
cd my-scriptbee-plugin
```

### 2. Install Native Federation

```bash
npm install --save-dev @angular-architects/native-federation@^21.2.3
npx ng add @angular-architects/native-federation --project my-scriptbee-plugin --type remote
```

### 3. Configure Native Federation

Replace the generated `federation.config.js` at the project root with the following, adjusting the `name` and `exposes`
entries for your plugin:

```js
const { withNativeFederation, shareAll } = require('@angular-architects/native-federation/config');

module.exports = withNativeFederation({
  name: 'my-scriptbee-plugin',
  exposes: {
    './Component': './src/app/my-plugin/my-plugin.ts',
    './routes': './src/app/app.routes.ts',
  },
  shared: {
    ...shareAll({ singleton: true, strictVersion: false, requiredVersion: 'auto' }),
  },
  skip: ['rxjs/ajax', 'rxjs/fetch', 'rxjs/testing', 'rxjs/webSocket'],
  features: {
    ignoreUnusedDeps: true,
  },
});
```

- `name` — must match the `remoteName` value in your `manifest.yaml`.
- `exposes` — maps keys (e.g. `./Component`, `./routes`) to source files. The keys are what you reference in
  `manifest.yaml` as `exposedModule`.

### 4. Update `src/main.ts`

Native Federation requires deferring the application bootstrap:

```typescript
import { initFederation } from '@angular-architects/native-federation';

initFederation()
  .catch(err => console.error(err))
  .then(_ => import('./bootstrap'))
  .catch(err => console.error(err));
```

Create `src/bootstrap.ts` to bootstrap the application:

```typescript
import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';

bootstrapApplication(App, appConfig).catch(err => console.error(err));
```

### 5. Plugin Manifest

Create a `manifest.yaml` in your plugin's distribution folder. The full structure is described
in [Plugin Manifest](manifest.md). For a UI plugin the extension point looks like:

```yaml
apiVersion: 1.0.0
name: My ScriptBee Plugin
description: A short description of what your plugin does
author: Your Name
extensionPoints:
  - kind: UI
    entryPoint: Plugin.dll
    version: 0.0.1
    remoteName: my-scriptbee-plugin
    remoteEntry: http://localhost:4201/remoteEntry.json
    outlets:
      - type: top-navigation-bar
        exposedModule: './routes'
        path: /my-plugin
        label: My Plugin
        nested: true
```

Replace the `outlets` block with the configuration for your chosen outlet type (see the sections below).

### 6. Build and serve

During development, serve the plugin locally:

```bash
ng serve
```

The default dev port is `4201`. Your `remoteEntry.json` will be available at `http://localhost:4201/remoteEntry.json`.

---

## Top Navigation Bar Outlet

A `top-navigation-bar` outlet adds a link to the ScriptBee global navigation bar. It is best suited for
standalone features or pages that users need access to from anywhere in the application.

### Manifest

```yaml
outlets:
  - type: top-navigation-bar
    exposedModule: './routes'
    path: /my-plugin
    label: My Plugin
    nested: true
```

| Field           | Required | Description                                                                                                                |
| --------------- | -------- | -------------------------------------------------------------------------------------------------------------------------- |
| `type`          | yes      | Must be `top-navigation-bar`                                                                                               |
| `exposedModule` | yes      | Key from `federation.config.js` `exposes` map, e.g. `'./routes'`                                                           |
| `path`          | yes      | URL path ScriptBee will register, e.g. `/my-plugin`                                                                        |
| `label`         | yes      | Text shown in the navigation bar                                                                                           |
| `nested`        | no       | If `true`, the exposed module's routes are mounted as children. If `false`, a single component is loaded. Default: `false` |
| `componentName` | no       | Name of the exported component when `nested` is `false`. Default: `App`                                                    |

### Angular Route Setup

When `nested: true`, the `exposedModule` must point to a file exporting a `Routes` array named `routes`:

```typescript
// src/app/app.routes.ts
import { Routes } from '@angular/router';
import { MyPluginComponent } from './my-plugin/my-plugin';

export const routes: Routes = [{ path: '', component: MyPluginComponent }];
```

When `nested: false`, ScriptBee dynamically loads the component referenced by `componentName` (default `App`)
from the `exposedModule`. In that case, point `exposedModule` to a file that exports that component:

```typescript
// src/app/my-plugin/my-plugin.ts
import { Component } from '@angular/core';

@Component({
  selector: 'app-my-plugin',
  imports: [],
  template: `<p>Hello from My Plugin!</p>`,
})
export class MyPlugin {}
```

---

## Side Panel Outlet

A `side-panel` outlet adds an icon-based entry to the ScriptBee side panel. Clicking the entry
navigates to a route or loads a component exposed by the plugin.

### Manifest

```yaml
outlets:
  - type: side-panel
    exposedModule: './Component'
    path: /my-plugin
    label: My Plugin
    icon: favorite
    nested: false
    componentName: App
```

| Field           | Required | Description                                                                                             |
| --------------- | -------- | ------------------------------------------------------------------------------------------------------- |
| `type`          | yes      | Must be `side-panel`                                                                                    |
| `exposedModule` | yes      | Key from `federation.config.js` `exposes` map, e.g. `'./Component'`                                     |
| `path`          | yes      | URL path ScriptBee will register                                                                        |
| `label`         | yes      | Text shown in the side panel tooltip                                                                    |
| `icon`          | yes      | A [Google Material Icon](https://fonts.google.com/icons) short name, e.g. `favorite`, `settings`, `add` |
| `nested`        | no       | If `true`, the exposed module's routes are mounted as children. Default: `false`                        |
| `componentName` | no       | Exported component name when `nested` is `false`. Default: `App`                                        |

### Angular Component Setup

```typescript
// src/app/my-plugin/my-plugin.ts
import { Component } from '@angular/core';

@Component({
  selector: 'app-my-plugin',
  imports: [],
  template: `<p>Side panel plugin content</p>`,
})
export class App {}
```

Expose it in `federation.config.js`:

```js
exposes: {
  './Component': './src/app/my-plugin/my-plugin.ts',
},
```

---

## File Previewer Outlet

A `file-previewer` outlet renders file content inside the ScriptBee output panel when the user opens
a file with a matching extension. ScriptBee passes the file metadata and its text content directly
to your component via Angular's `inputs` binding.

### Manifest

```yaml
outlets:
  - type: file-previewer
    exposedModule: './Component'
    label: My Plugin Previewer
    icon: preview
    supportedFileExtensions:
      - json
      - txt
    componentName: App
```

| Field                     | Required | Description                                                                                 |
| ------------------------- | -------- | ------------------------------------------------------------------------------------------- |
| `type`                    | yes      | Must be `file-previewer`                                                                    |
| `exposedModule`           | yes      | Key from `federation.config.js` `exposes` map, e.g. `'./Component'`                         |
| `label`                   | yes      | Text shown in the file preview tab header                                                   |
| `icon`                    | no       | A [Google Material Icon](https://fonts.google.com/icons) short name                         |
| `supportedFileExtensions` | no       | List of file extensions (without `.`) this previewer handles. If empty, shown for all files |
| `componentName`           | no       | Exported component name. Default: `App`                                                     |

### Receiving File Content

ScriptBee passes data to your component using Angular's `*ngComponentOutlet` `inputs` binding:

```typescript
inputs: {
  content: string;
  file: AnalysisFile;
  theme: 'light' | 'dark';
}
```

Your component **must** declare inputs with the same names and compatible types.

```typescript
// src/app/my-plugin/my-plugin.ts
import { Component, input } from '@angular/core';

interface AnalysisFile {
  id: string;
  name: string;
  type: string;
}

@Component({
  selector: 'app-my-plugin',
  imports: [],
  template: `
    <h3>{{ file()?.name }}</h3>
    <pre>{{ content() }}</pre>
  `,
})
export class App {
  content = input.required<string>();
  file = input<AnalysisFile>();
  theme = input<string>();
}
```

Expose the component in `federation.config.js`:

```js
export default withNativeFederation({
    exposes: {
        './Component': './src/app/my-plugin/my-plugin.ts',
    }
)
```

---

## Further Reading

- [UI Plugin reference](ui_plugin.md) — full outlet field reference
- [Plugin Manifest](manifest.md) — full manifest structure
- [Angular Native Federation](https://www.angulararchitects.io/en/blog/combining-native-federation-and-module-federation/)
