using Microsoft.CodeAnalysis;

namespace UaDetector.SourceGenerator.Utilities;

public static class RoslynExtensions
{
    public static string ToSyntaxString(this Accessibility accessibility) =>
        accessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Private => "private",
            Accessibility.Internal => "internal",
            Accessibility.Protected => "protected",
            _ => throw new NotSupportedException(),
        };

    public static IEnumerable<ISymbol> GetMembersRecursively(this INamespaceOrTypeSymbol symbol)
    {
        foreach (var member in symbol.GetMembers())
        {
            yield return member;

            if (member is INamespaceOrTypeSymbol nested)
            {
                foreach (var nestedMember in GetMembersRecursively(nested))
                    yield return nestedMember;
            }
        }
    }
}
