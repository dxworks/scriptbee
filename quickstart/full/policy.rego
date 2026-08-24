package scriptbee.auth

import data.scriptbee.auth.roles_and_permissions.project_permissions

default allow = false

allow if {
    role := input.resource.role
    project_permissions[role][input.action]
}

allow if {
    input.action == "project:create"
}

user_permissions contains perm if {
   role := input.resource.role   
   perm := project_permissions[role][_] 
}
