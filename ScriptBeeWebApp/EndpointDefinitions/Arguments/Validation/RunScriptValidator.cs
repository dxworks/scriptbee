﻿using FluentValidation;

namespace ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;

public class RunScriptValidator : AbstractValidator<RunScript>
{
    public RunScriptValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();
        RuleFor(x => x.FilePath)
            .NotEmpty();
        RuleFor(x => x.Language)
            .NotEmpty();
    }
}
