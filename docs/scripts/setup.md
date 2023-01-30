# Setup

ScriptBee has a default bundle that contains script generators and runners for C#, Python and Javascript.

A new script can be created using the `New Script` button in the `Scripts` section.

Writing the script is done by updating the file manually. ScriptBee offers a read-only view of the script.

## Visual Studio Code

ScriptBee has a button that automatically opens the script in Visual Studio Code. Saving the modified script, will
automatically update the preview.

In order to work properly, the `UserFolder__UserFolderPath` environment variable must be set correctly.

See [User Folder Setup](../home/installation.md#user-folder-setup) section for more information.

## Code Generation

ScriptBee will generate classes for the entities of the context. The generated code can be found in the `generated` folder, next to the `src` folder.
