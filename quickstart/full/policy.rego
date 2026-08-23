package scriptbee.auth

default allow = false

allow if {
    role := input.resource.role
    project_permissions[role][input.action]
}

allow if {
    input.action == "project:create"
}

project_permissions := {
    "Admin": {
        "project:view",
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
        "analysis:view",
        "analysis:run",
        "analysis:delete",
        "plugin:view",
        "plugin:install",
        "plugin:uninstall",
        "plugin:configure",
        "token:create",
        "token:delete",
        "gateway_plugin:view",
        "gateway_plugin:install",
        "gateway_plugin:uninstall",
        "gateway_plugin:configure"
    },
    "Editor": {
        "project:view",
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
        "script:view",
        "model:view",
        "model:upload",
        "model:load",
        "model:link",
        "model:clear",
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
