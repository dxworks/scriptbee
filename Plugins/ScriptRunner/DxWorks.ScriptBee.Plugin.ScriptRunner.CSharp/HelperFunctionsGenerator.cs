using System.Reflection;
using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Services;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp;

public static class HelperFunctionsGenerator
{
    // https://roslynquoter.azurewebsites.net/

    public static SyntaxList<MemberDeclarationSyntax> GetMemberDeclarationSyntaxList(
        IHelperFunctionsContainer helperFunctionsContainer)
    {
        var syntaxTree = CreateSyntaxTree(helperFunctionsContainer);

        return (((syntaxTree.GetRoot() as CompilationUnitSyntax)!
                .Members.First() as NamespaceDeclarationSyntax)!
            .Members.First() as ClassDeclarationSyntax)!.Members;
    }

    public static SyntaxTree CreateSyntaxTree(IHelperFunctionsContainer helperFunctionsContainer)
    {
        var helperFunctionTypes = helperFunctionsContainer.GetFunctions()
            .Select(f => f.GetType())
            .ToList();

        var fieldDeclarationSyntaxes = helperFunctionTypes
            .Select(t => FieldDeclaration(
                    VariableDeclaration(IdentifierName(t.FullName ?? t.Name))
                        .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier(t.Name)))))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))));

        var methodDeclarationSyntaxes = helperFunctionTypes
            .SelectMany(t => t.GetMethods().Where(m => m.DeclaringType == t))
            .Select(methodInfo =>
            {
                if (methodInfo.ReturnType == typeof(void))
                {
                    return GenerateVoidMethodDeclaration(methodInfo);
                }

                return GenerateMethodDeclarationWithReturnValue(methodInfo);
            });

        var classDeclaration = ClassDeclaration(nameof(HelperFunctions))
            .WithMembers(List(fieldDeclarationSyntaxes.Concat(methodDeclarationSyntaxes)))
            .WithModifiers(TokenList(Token(SyntaxKind.StaticKeyword), Token(SyntaxKind.PartialKeyword)));

        var compilationUnit = CompilationUnit()
            .WithMembers(SingletonList<MemberDeclarationSyntax>(
                NamespaceDeclaration(IdentifierName(typeof(HelperFunctions).Namespace))
                    .WithMembers(SingletonList<MemberDeclarationSyntax>(classDeclaration))))
            .WithUsings(GenerateUsings())
            .NormalizeWhitespace();

        return compilationUnit.SyntaxTree;
    }

    private static SyntaxList<UsingDirectiveSyntax> GenerateUsings()
    {
        return List(new List<UsingDirectiveSyntax>
        {
            UsingDirective(IdentifierName("System"))
        });
    }

    private static MemberDeclarationSyntax GenerateMethodDeclarationWithReturnValue(MethodInfo methodInfo)
    {
        return GenerateMethodSignature(methodInfo)
            .WithBody(Block(SingletonList<StatementSyntax>(ReturnStatement(
                InvocationExpression(
                        GenerateHelperFunctionMethodCall(methodInfo))
                    .WithArgumentList(
                        ArgumentList(
                            SeparatedList(
                                methodInfo.GetParameters().Select(parameterInfo =>
                                    Argument(IdentifierName(parameterInfo.Name))))))))));
    }

    private static MethodDeclarationSyntax GenerateVoidMethodDeclaration(MethodInfo methodInfo)
    {
        return GenerateMethodSignature(methodInfo)
            .WithBody(Block(SingletonList<StatementSyntax>(ExpressionStatement(
                InvocationExpression(
                        GenerateHelperFunctionMethodCall(methodInfo))
                    .WithArgumentList(
                        ArgumentList(
                            SeparatedList(
                                methodInfo.GetParameters().Select(parameterInfo =>
                                    Argument(IdentifierName(parameterInfo.Name))))))))));
    }

    private static MethodDeclarationSyntax GenerateMethodSignature(MethodInfo methodInfo)
    {
        var methodDeclarationSyntax =
            MethodDeclaration(GenerateReturnValue(methodInfo.ReturnParameter), methodInfo.Name)
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                .WithParameterList(ParameterList(SeparatedList(
                        methodInfo.GetParameters().Select(parameterInfo => Parameter(Identifier(parameterInfo.Name))
                            .WithType(GenerateParameter(parameterInfo)))
                    )
                ));

        var typeParameterSyntaxes = methodInfo.GetGenericArguments()
            .Select(type => TypeParameter(Identifier(type.Name)))
            .ToList();

        if (!typeParameterSyntaxes.Any())
        {
            return methodDeclarationSyntax;
        }

        var hasGenericConstrains = methodInfo.GetGenericArguments()
            .Any(x => x.GetGenericParameterConstraints().Any());

        var methodDeclarationWithGenericParameters = methodDeclarationSyntax
            .WithTypeParameterList(TypeParameterList(SeparatedList(typeParameterSyntaxes)));


        if (!hasGenericConstrains)
        {
            return methodDeclarationWithGenericParameters;
        }

        var genericConstraintsSyntaxes = methodInfo.GetGenericArguments()
            .Select(type => (type, GenerateGenericConstrains(type).ToList()))
            .Where(tuple => tuple.Item2.Any())
            .Select(tuple =>
            {
                var (type, genericConstrains) = tuple;
                return TypeParameterConstraintClause(type.Name)
                    .WithConstraints(
                        SeparatedList<TypeParameterConstraintSyntax>(
                            genericConstrains.Select(x => TypeConstraint(IdentifierName(x)))));
            })
            .ToList();

        return methodDeclarationWithGenericParameters
            .WithConstraintClauses(List(genericConstraintsSyntaxes));
    }

    private static IEnumerable<string> GenerateGenericConstrains(Type type)
    {
        var genericParameterConstraints = type.GetGenericParameterConstraints();
        foreach (var genericParameterConstraint in genericParameterConstraints)
        {
            var constraintName = genericParameterConstraint.FullName ?? genericParameterConstraint.Name;
            if (constraintName == typeof(ValueType).FullName)
            {
                yield return "struct";
            }
            else
            {
                yield return constraintName;
            }
        }

        if (genericParameterConstraints.Any())
        {
            yield break;
        }

        if (type.GenericParameterAttributes.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint))
        {
            yield return "class";
        }

        if (type.GenericParameterAttributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint))
        {
            yield return "new()";
        }

        if (type.GenericParameterAttributes.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint))
        {
            yield return "struct";
        }
    }

    private static MemberAccessExpressionSyntax GenerateHelperFunctionMethodCall(MethodBase methodInfo)
    {
        SimpleNameSyntax nameSyntax = !methodInfo.IsGenericMethod
            ? IdentifierName(methodInfo.Name)
            : GenericName(methodInfo.Name)
                .WithTypeArgumentList(TypeArgumentList(SeparatedList(
                    methodInfo.GetGenericArguments().Select(t => IdentifierName(t.Name)).OfType<TypeSyntax>()
                )));

        return MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName(methodInfo.DeclaringType.Name),
            nameSyntax);
    }

    private static TypeSyntax GenerateParameter(ParameterInfo parameterInfo)
    {
        var parameterSyntax = GetParameterSyntax(parameterInfo.ParameterType);

        if (IsParameterNullable(parameterInfo))
        {
            return NullableType(parameterSyntax);
        }

        return parameterSyntax;
    }

    private static TypeSyntax GenerateReturnValue(ParameterInfo returnTypeParameterInfo)
    {
        var returnType = returnTypeParameterInfo.ParameterType;

        var returnTypeSyntax = GetParameterSyntax(returnType);

        if (IsParameterNullable(returnTypeParameterInfo))
        {
            return NullableType(returnTypeSyntax);
        }

        return returnTypeSyntax;
    }


    private static NameSyntax GetParameterSyntax(Type parameterType)
    {
        var parameterTypeFullName = parameterType.FullName ?? parameterType.Name;

        NameSyntax parameterSyntax;
        if (parameterTypeFullName.Contains('`'))
        {
            parameterTypeFullName = parameterType.ToString();
            var index = parameterTypeFullName.IndexOf('`');
            parameterTypeFullName = parameterTypeFullName[..index];

            parameterSyntax = GenericName(parameterTypeFullName)
                .WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(
                    parameterType.GetGenericArguments().Select(GetParameterSyntax)
                )));
        }
        else
        {
            parameterTypeFullName = ConvertSystemTypeToPrimitiveType(parameterTypeFullName);

            parameterSyntax = IdentifierName(parameterTypeFullName);
        }

        return parameterSyntax;
    }

    private static bool IsParameterNullable(ParameterInfo parameterInfo)
    {
        var nullabilityInfo = new NullabilityInfoContext().Create(parameterInfo);

        return nullabilityInfo.ReadState == NullabilityState.Nullable ||
               nullabilityInfo.WriteState == NullabilityState.Nullable;
    }

    private static string ConvertSystemTypeToPrimitiveType(string type)
    {
        return type switch
        {
            "System.Byte" => "byte",
            "System.SByte" => "sbyte",
            "System.Int32" => "int",
            "System.UInt32" => "uint",
            "System.Int16" => "short",
            "System.UInt16" => "ushort",
            "System.Int64" => "long",
            "System.UInt64" => "ulong",
            "System.Single" => "float",
            "System.Double" => "double",
            "System.Char" => "char",
            "System.Boolean" => "bool",
            "System.Object" => "object",
            "System.String" => "string",
            "System.Decimal" => "decimal",
            "System.DateTime" => "DateTime",
            "System.Void" => "void",
            _ => type
        };
    }
}
