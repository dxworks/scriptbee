package scriptbee.auth.roles_and_permissions

import rego.v1

roles := [
    {"id": "Editor", "description": "User that can manage projects they belong to."},
    {"id": "Analyst", "description": "User that can perform analysis tasks on allowed projects."},
    {"id": "Viewer", "description": "User that can view only allowed projects and their analysis."}
]

default_creator_role := "Editor"

plugin_marketplace_permissions := {
   "plugin:view"
}

project_permissions := {
    "Admin": {
        "project:view",
        "project:live_updates",
        "project:edit",
        "project:delete",
        "script:view",
        "script:create",
        "script:edit",
        "script:delete",
        "model:view",
        "model:upload",
        "model:load",
        "model:link",
        "model:clear",
        "instance:view",
        "instance:allocate",
        "instance:deallocate",
        "analysis:view",
        "analysis:run",
        "analysis:delete",
        "plugin:view",
        "plugin:install",
        "plugin:uninstall",
        "plugin:configure",
        "token:create",
        "token:delete"
    },
    "Editor": {
        "project:view",
        "project:live_updates",
        "project:edit",
        "project:delete",
        "script:view",
        "script:create",
        "script:edit",
        "script:delete",
        "model:view",
        "model:upload",
        "model:load",
        "model:link",
        "model:clear",
        "instance:view",
        "instance:allocate",
        "instance:deallocate",
        "analysis:view",
        "analysis:run",
        "analysis:delete",
        "plugin:view",
        "plugin:install",
        "plugin:uninstall",
        "plugin:configure",
        "token:create",
        "token:delete"
    },
    "Analyst": {
        "project:view",
        "project:live_updates",
        "script:view",
        "model:view",
        "model:upload",
        "model:load",
        "model:link",
        "model:clear",
        "instance:view",
        "instance:allocate",
        "instance:deallocate",
        "analysis:view",
        "analysis:run",
        "plugin:view"
    },
    "Viewer": {
        "project:view",
        "script:view",
        "analysis:view"
    }
}
