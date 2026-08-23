package scriptbee.auth.roles

import rego.v1

roles := [
    {"id": "Admin", "description": "User that can perform any action."},
    {"id": "Editor", "description": "User that can manage projects they belong to."},
    {"id": "Analyst", "description": "User that can perform analysis tasks on allowed projects."},
    {"id": "Viewer", "description": "User that can view only allowed projects and their analysis."}
]

default_creator_role := "Editor"
