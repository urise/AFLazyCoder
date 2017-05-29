namespace BusinessLayer.CodeClasses
{
    public interface IMethodData
    {
        string FullSignature { get; }
        string ReturnType { get; }
        string MethodName { get; }
        string Parameters { get; }
        string ParametersWithoutTypes { get; }
        bool IsValid { get; }
    }
}