using DxWorks.ScriptBee.Plugin.Api.Model;
using DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Exceptions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp;

public static class ScriptParametersGenerator 
{
    public static ClassDeclarationSyntax GenerateScriptParameters(IEnumerable<ScriptParameter> parameters)
    {
        return ClassDeclaration("ScriptParameters")
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddMembers(GenerateProperties(parameters))
            .NormalizeWhitespace();
    }

    private static MemberDeclarationSyntax[] GenerateProperties(IEnumerable<ScriptParameter> parameters)
    {
        var members = new List<MemberDeclarationSyntax>();

        foreach (var scriptParameter in parameters)
        {
            members.Add(GenerateProperty(scriptParameter));
        }

        return members.ToArray();
    }

    private static MemberDeclarationSyntax GenerateProperty(ScriptParameter scriptParameter)
    {
        var property = PropertyDeclaration(ParseTypeName(scriptParameter.Type), scriptParameter.Name)
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddAccessorListAccessors(
                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));

        if (scriptParameter.Value != null)
        {
            property = property.WithInitializer(EqualsValueClause(
                    GenerateLiteralExpression(scriptParameter.Type, scriptParameter.Value)))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }

        return property;
    }

    private static LiteralExpressionSyntax GenerateLiteralExpression(string type, string value)
    {
        return type switch
        {
            ScriptParameter.TypeString => LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(value)),
            ScriptParameter.TypeInteger => LiteralExpression(SyntaxKind.NumericLiteralExpression,
                Literal(int.Parse(value))),
            ScriptParameter.TypeBoolean => GetBooleanExpressionSyntax(value),
            ScriptParameter.TypeFloat => LiteralExpression(SyntaxKind.NumericLiteralExpression,
                Literal(float.Parse(value))),
            _ => throw new InvalidParameterTypeException(type)
        };
    }

    private static LiteralExpressionSyntax GetBooleanExpressionSyntax(string value)
    {
        return value switch
        {
            "true" => LiteralExpression(SyntaxKind.TrueLiteralExpression),
            _ => LiteralExpression(SyntaxKind.FalseLiteralExpression)
        };
    }
}
