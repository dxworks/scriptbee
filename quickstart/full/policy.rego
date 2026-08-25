package scriptbee.auth

import data.scriptbee.auth.roles_and_permissions.project_permissions
import data.scriptbee.auth.roles_and_permissions.plugin_marketplace_permissions

default allow = false

allow if {
    role := input.resource.role
    project_permissions[role][input.action]
}

allow if {
    input.action == "project:create"
}

allow if {
    input.resource.type == "global"
    plugin_marketplace_permissions[input.action]
}

user_permissions contains perm if {
   role := input.resource.role   
   perm := project_permissions[role][_] 
}
