using System.ComponentModel;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using ScriptBee.MCP.Gateway.Generated;

namespace ScriptBee.MCP.Prompts;

[McpServerPromptType]
public sealed class ScriptBeePrompts
{
    [McpServerPrompt(Name = "run-analysis")]
    [Description(
        "Guides an AI through loading context and running a ScriptBee analysis script on a project instance."
    )]
    public GetPromptResult RunAnalysis(
        [Description("The ID of the ScriptBee project to analyse.")] string projectId,
        [Description("The ID of the instance to use (create one with CreateInstance if unsure).")]
            string instanceId,
        [Description("The ID of the script to execute.")] string scriptId
    ) =>
        new()
        {
            Description =
                "Guides an AI through loading context and running a ScriptBee analysis script on a project instance.",
            Messages =
            [
                new PromptMessage
                {
                    Role = Role.User,
                    Content = new TextContentBlock
                    {
                        Text = $"""
                            Please run a ScriptBee analysis for me using the following details:
                            - Project ID: {projectId}
                            - Instance ID: {instanceId}
                            - Script ID: {scriptId}

                            Steps to follow:
                            1. Verify the project exists using GetProject.
                            2. Check the current context with GetContext. If the context is empty, list loaders with GetLoaders and call LoadContext, then list linkers with GetLinkers and call LinkContext.
                            3. Trigger the analysis with TriggerAnalysis using the project, instance and script IDs provided.
                            4. Poll GetAnalysisStatus until the status is no longer 'Running'.
                            5. Report the final status. If it succeeded, show the first lines of GetAnalysisConsole. If it failed, show the errors from GetAnalysisErrors.
                            """,
                    },
                },
            ],
        };

    [McpServerPrompt(Name = "explore-project")]
    [Description(
        "Gives an AI a structured starting point for exploring a ScriptBee project's scripts, instances, and recent analyses."
    )]
    public GetPromptResult ExploreProject(
        [Description("The ID of the ScriptBee project to explore.")] string projectId
    ) =>
        new()
        {
            Description =
                "Gives an AI a structured starting point for exploring a ScriptBee project.",
            Messages =
            [
                new PromptMessage
                {
                    Role = Role.User,
                    Content = new TextContentBlock
                    {
                        Text = $"""
                            Please give me an overview of ScriptBee project '{projectId}':
                            1. Fetch the project details with GetProject.
                            2. List all scripts with GetScripts and summarise them (name, language, path).
                            3. List all instances with GetInstances and show their status.
                            4. List the five most recent analyses with GetAnalyses (sort by createdAt:desc) and show their status and script used.
                            """,
                    },
                },
            ],
        };

    [McpServerPrompt(Name = "load-and-link-context")]
    [Description(
        "Guides an AI through the full context loading and linking workflow for a project instance."
    )]
    public GetPromptResult LoadAndLinkContext(
        [Description("The ID of the ScriptBee project.")] string projectId,
        [Description("The ID of the instance to load context into.")] string instanceId
    ) =>
        new()
        {
            Description =
                "Guides an AI through the full context loading and linking workflow for a project instance.",
            Messages =
            [
                new PromptMessage
                {
                    Role = Role.User,
                    Content = new TextContentBlock
                    {
                        Text = $"""
                            Please load and link the full data context for project '{projectId}', instance '{instanceId}':
                            1. Fetch available loaders with GetLoaders.
                            2. Call LoadContext with all available loader IDs.
                            3. Fetch available linkers with GetLinkers.
                            4. Call LinkContext with all available linker IDs.
                            5. Confirm by calling GetContext and summarising the loaded model.
                            """,
                    },
                },
            ],
        };
}
