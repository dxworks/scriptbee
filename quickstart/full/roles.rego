package scriptbee.auth.roles

import rego.v1

roles := [
    {"id": "Administrator", "description": "User that can perform any action."},
    {"id": "Manager", "description": "User that can manage projects they belong to."},
    {"id": "Analyst", "description": "User that can perform analysis tasks on allowed projects."},
    {"id": "Auditor", "description": "User that can view only allowed projects."}
]
