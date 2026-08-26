export type Permission =
  | 'project:create'
  | 'project:view'
  | 'project:edit'
  | 'project:delete'
  | 'project:manage_access'
  | 'script:view'
  | 'script:create'
  | 'script:edit'
  | 'script:delete'
  | 'model:view'
  | 'model:upload'
  | 'model:load'
  | 'model:link'
  | 'model:clear'
  | 'instance:view'
  | 'instance:allocate'
  | 'instance:deallocate'
  | 'analysis:view'
  | 'analysis:run'
  | 'analysis:delete'
  | 'plugin:view'
  | 'plugin:install'
  | 'plugin:uninstall'
  | 'plugin:configure'
  | 'token:create'
  | 'token:delete'
  | 'gateway_plugin:management';

export interface GlobalPermissionsResponse {
  permissions: string[];
}
