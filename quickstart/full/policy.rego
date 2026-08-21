package scriptbee.auth

default allow = false

allow {
    role := input.resource.userRole
    
    allowed_permissions := role_permissions[role]
    
    allowed_permissions[input.action]
}

role_permissions := {
    "Administrator": {
        "project:view",
        "project:edit",
        "project:remove",
        "project:load_model",
        "project:link_model",
        "project:generate_script",
        "project:create_script",
        "project:edit_script",
        "project:delete_script",
        "analysis:view"
    },
    "Manager": {
        "project:view",
        "project:edit",
        "project:remove",
        "project:load_model",
        "project:link_model",
        "project:generate_script",
        "project:create_script",
        "project:edit_script",
        "project:delete_script",
        "analysis:view",
        "analysis:run",
        "analysis:remove"
    },
    "Analyst": {
        "project:view",
        "project:load_model",
        "project:link_model",
        "project:generate_script",
        "project:create_script",
        "project:edit_script",
        "project:delete_script",
        "analysis:view",
        "analysis:run",
        "analysis:remove",
        "analysis_token:create",
        "analysis_token:delete"
    },
    "Auditor": {
        "project:view",
        "analysis:view",
        "analysis:run",
        "analysis:remove",
        "analysis_token:create",
        "analysis_token:delete"
    }
}
