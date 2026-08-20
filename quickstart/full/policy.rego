package rbac

import rego.v1

default allow := false

role_permissions := {
    "admin":  ["documents:read", "documents:write", "documents:delete"],
    "editor": ["documents:read", "documents:write"],
    "viewer": ["documents:read"]
}

allow if {
    some role in input.roles
    input.action in role_permissions[role]
}
