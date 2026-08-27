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
    input.subject.groups[_] == "admin"
}

allow if {
    input.resource.type == "global"
    plugin_marketplace_permissions[input.action]
}

allow if {
    input.resource.type == "global"
    input.action == "gateway_plugin:management"
    input.subject.groups[_] == "admin"
}

user_permissions contains perm if {
   role := input.resource.role   
   perm := project_permissions[role][_] 
}

user_permissions contains "project:create" if {
    input.resource.type == "global"
}

user_permissions contains "project:view_all" if {
    input.resource.type == "global"
    input.subject.groups[_] == "admin"
}

user_permissions contains perm if {
    input.resource.type == "global"
    perm := plugin_marketplace_permissions[_]
}

user_permissions contains "gateway_plugin:management" if {
    input.resource.type == "global"
    input.subject.groups[_] == "admin"
}
