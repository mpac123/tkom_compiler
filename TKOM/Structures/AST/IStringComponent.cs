using TKOM.Structures.IR;

namespace TKOM.Structures.AST
{
    public interface IStringComponent : IInstruction
    {
        string GetValue(Scope scope);
    }
}