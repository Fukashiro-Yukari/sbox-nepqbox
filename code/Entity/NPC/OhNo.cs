using Sandbox;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

[Spawnable]
[Library( "npc_ohno", Title = "Oh No S&box" )]
public partial class SayOhNo : NPC
{
	public override float SpawnHealth => 100;
	public override float Speed => 200;

	public override void Spawn()
	{
		base.Spawn();

		var wander = new Sandbox.Nav.Wander();
		wander.MinRadius = 500;
		wander.MaxRadius = 2000;

		Steer = wander;
	}

	TimeSince timeSinceSay;

	public override void OnTick()
	{
		base.OnTick();

		if ( timeSinceSay > 5f )
		{
			timeSinceSay = 0;

			var love = new LoveTextEntity()
			{
				Position = Position + Vector3.Up * 80
			};

			love.Velocity = EyeRotation.Up * 500;
			love.DeleteAsync( 10f );

			var random = new Random();
			var sound = "oh_no";

			if ( random.NextSingle() < 0.3 )
				sound = "no_no_no";

			PlaySound( sound );
		}
	}
}
