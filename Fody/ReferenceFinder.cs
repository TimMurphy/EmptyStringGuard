using System.Linq;
using Mono.Cecil;

public static class ReferenceFinder
{
    public static MethodReference ArgumentExceptionConstructor;
    public static MethodReference StringEqualityMethod;
    public static FieldReference EmptyStringField;
    public static MethodReference DebugAssertMethod;

    public static void FindReferences(IAssemblyResolver assemblyResolver, ModuleDefinition moduleDefinition)
    {
        var baseLib = assemblyResolver.Resolve("mscorlib");
        var baseLibTypes = baseLib.MainModule.Types;

        var winrt = !baseLibTypes.Any(type => type.Name == "Object");
        if (winrt)
        {
            baseLib = assemblyResolver.Resolve("System.Runtime");
            baseLibTypes = baseLib.MainModule.Types;
        }

        var argumentException = baseLibTypes.First(x => x.Name == "ArgumentException");
        ArgumentExceptionConstructor = moduleDefinition.Import(argumentException.Methods.First(x =>
            x.IsConstructor &&
            x.Parameters.Count == 2 &&
            x.Parameters[0].ParameterType.Name == "String" &&
            x.Parameters[1].ParameterType.Name == "String"));

        var debugLib = !winrt ? assemblyResolver.Resolve("System") : assemblyResolver.Resolve("System.Diagnostics.Debug");
        var debugLibTypes = debugLib.MainModule.Types;

        var debug = debugLibTypes.First(x => x.Name == "Debug");
        DebugAssertMethod = moduleDefinition.Import(debug.Methods.First(x =>
            x.IsStatic &&
            x.Parameters.Count == 2 &&
            x.Parameters[0].ParameterType.Name == "Boolean" &&
            x.Parameters[1].ParameterType.Name == "String"));

        var stringTypeDefinition = moduleDefinition.TypeSystem.String.Resolve();
        StringEqualityMethod = moduleDefinition.Import(stringTypeDefinition.Methods.First(x => x.Name == "op_Equality"));
        EmptyStringField = moduleDefinition.Import(stringTypeDefinition.Fields.First(x => x.Name == "Empty"));
    }
}