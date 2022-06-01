using Sandbox;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

[Spawnable]
[Library("npc_follow", Title = "Follow Player Npc" )]
public partial class NPCFollow : NPC
{
	public override float SpawnHealth => 100;

	Player Target;

	private void FindTarget()
	{
		var rply = All.OfType<Player>().ToArray();

		Target = rply[Rand.Int( 0, rply.Count() - 1 )];
	}

	public override void OnTick()
	{
		if ( Target == null || Target.LifeState == LifeState.Dead )
			FindTarget();
		else if ( Target.Position.Distance( Position ) > 100 )
		{
			Steer = new NavSteer();
			Steer.Target = Target.Position;
			Steer.DontAvoidance = e => e.Parent == Target || !e.EnableDrawing || e == this;
		}
		else
		{
			Steer = null;
		}
	}
}
