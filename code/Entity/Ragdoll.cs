using Sandbox;

[Spawnable]
[Library( "ent_ragdoll", Title = "Ragdoll" )]
public partial class Ragdoll : Prop, IUse
{
	Player Player;
	bool IsUsing;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/citizen/citizen.vmdl" );
		Health = 1;
	}

	public bool IsUsable( Entity user )
	{
		return !IsUsing && user is SandboxPlayer ply && !ply.Ragdoll.IsValid();
	}

	public bool OnUse( Entity user )
	{
		if ( user is SandboxPlayer ply )
		{
			Player = ply;
			ply.Ragdoll = this;

			foreach ( var child in ply.Children )
			{
				if ( !child.Tags.Has( "clothes" ) ) continue;
				if ( child is not ModelEntity e ) continue;

				var model = e.GetModelName();

				var clothing = new ModelEntity();
				clothing.SetModel( model );
				clothing.SetParent( this, true );
				clothing.RenderColor = e.RenderColor;
				clothing.CopyBodyGroups( e );
				clothing.CopyMaterialGroup( e );
			}

			IsUsing = true;
		}

		return false;
	}

	public override void TakeDamage( DamageInfo info )
	{
		info.Damage *= 2f;

		if ( Player.IsValid() )
			Player.TakeDamage( info );
	}

	private bool IsDelete;

	private void ToDelete()
	{
		IsDelete = true;
		DeleteAsync( 10f );
	}

	[Event.Tick.Server]
	public void OnServerTick()
	{
		if ( IsUsing && !IsDelete )
		{
			if ( Player.IsValid() )
			{
				if ( Player.LifeState != LifeState.Alive )
					ToDelete();
			}
			else
				ToDelete();
		}
	}
}
