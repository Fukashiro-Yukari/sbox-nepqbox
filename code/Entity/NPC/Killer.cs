using Sandbox;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

[Spawnable]
[Library( "npc_killer", Title = "Killer" )]
public partial class Killer : NPC
{
	public override float SpawnHealth => 200;
	public override float Speed => 300;

	Entity Target;

	private void FindTarget()
	{
		var rnpc = All.OfType<NPC>().Where( x => x != this && x.IsValid() ).ToArray();

		if ( rnpc.Count() < 1 )
		{
			if ( Steer is not Sandbox.Nav.Wander )
				Steer = null;

			Target = null;

			return;
		}

		Target = rnpc[Rand.Int( 0, rnpc.Count() - 1 )];
	}

	public override void OnTick()
	{
		if ( !Target.IsValid() || Target.LifeState != LifeState.Alive )
		{
			FindTarget();

			if ( Steer == null )
			{
				NowSpeed = 50;

				var wander = new Sandbox.Nav.Wander();
				wander.MinRadius = 500;
				wander.MaxRadius = 800;

				Steer = wander;
			}
		}
		else
		{
			NowSpeed = Speed;

			var steer = new NavSteer();
			steer.Target = Target.Position;
			steer.DontAvoidance = e => e.Parent == Target || !e.EnableDrawing || e == this;

			Steer = steer;
		}
	}

	public override void DoMeleeStrike()
	{
		if ( !Target.IsValid() || Target.LifeState != LifeState.Alive ) return;
		if ( Target.Position.Distance( Position ) < 100 )
		{
			MeleeStrike( 20, 2f );
		}
	}

	public override void TakeDamage( DamageInfo info )
	{
		base.TakeDamage( info );

		if ( !Target.IsValid() && info.Attacker.Health != -1 )
			Target = info.Attacker;
	}
}
