using Sandbox;

[Library( "npc_dummy", Title = "Dummy", Spawnable = true )]
public partial class NPCDummy : NPC
{
	public override float SpawnHealth => 100;

	[Event.Frame]
	public void OnFrame()
	{
		DebugOverlay.Text( EyePosition, Health.ToString() );
	}

	public override void TakeDamage( DamageInfo info )
	{
		base.TakeDamage( info );

		var damage = info.Damage;

		if ( GetHitboxGroup( info.HitboxIndex ) == 1 )
		{
			damage *= 2.0f;
		}

		Log.Info( $"Dummy Take Damage: {damage}" );
	}
}
