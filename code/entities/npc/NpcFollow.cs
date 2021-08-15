using Sandbox;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

[Library("npc_follow", Title = "Follow Player Npc", Spawnable = true )]
public partial class NpcFollow : Npc
{
	public override float InitHealth => 100;

	SandboxPlayer Target;

	private void FindTarget()
    {
		var rply = All.OfType<SandboxPlayer>().ToArray();

		Target = rply[Rand.Int( 0, rply.Count() - 1 )];
    }

	public override void OnTick()
	{
		if (Target == null || Target.LifeState == LifeState.Dead)
			FindTarget();
		else if (Target.Position.Distance(Position) > 100)
        {
			Steer = new NavSteer();
			Steer.Target = Target.Position;
			Steer.DontAvoidance = e => e.Parent == Target || e is Weapon || e.Parent is Weapon;
		}
		else
        {
			Steer = null;
		}
	}
}
