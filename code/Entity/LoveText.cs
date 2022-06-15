using Sandbox;
using System;
using System.Threading.Tasks;

[Spawnable]
[Library( "ent_lovetext", Title = "I Love S&box" )]
public partial class LoveTextEntity : Prop, IUse
{
	private LoveTextPanel panel;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/ball/ball.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
		Scale = 2;
		EnableDrawing = false;
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		panel = new LoveTextPanel();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		if ( IsClient )
			panel.Delete();
	}

	[Event.Tick.Client]
	public void OnClientTick()
	{
		panel.Transform = Transform;
	}

	public bool IsUsable( Entity user )
	{
		return true;
	}

	public bool OnUse( Entity user )
	{
		var random = new Random();
		var sound = "oh_no";

		if ( random.NextSingle() < 0.3 )
			sound = "no_no_no";

		PlaySound( sound );
		user.PlaySound( sound );

		return false;
	}

	protected override void OnPhysicsCollision( CollisionEventData eventData )
	{
		base.OnPhysicsCollision( eventData );

		if ( eventData.Speed < 100 )
			return;

		Log.Info( eventData.Speed );

		var random = new Random();
		var sound = "oh_no";

		if ( random.NextSingle() < 0.3 )
			sound = "no_no_no";

		PlaySound( sound );
	}
}
