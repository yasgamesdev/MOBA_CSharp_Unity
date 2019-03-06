using MOBA_CSharp_Server.Library.ECS;

namespace MOBA_CSharp_Server.Game
{
    public class UnitComponent : Entity
    {
        public Unit unitRoot { get; private set; }

        public UnitComponent(Unit unitRoot, Entity root) : base(root)
        {
            AddInheritedType(typeof(UnitComponent));

            this.unitRoot = unitRoot;
        }
    }
}
