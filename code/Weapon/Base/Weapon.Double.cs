using Sandbox;

public partial class WeaponDouble : Weapon
{
	public virtual string MuzzleLeftAttachment => "muzzle_left";
	public virtual string MuzzleRightAttachment => "muzzle_right";
	public virtual string EjectionPointLeftAttachment => "ejection_point_left";
	public virtual string EjectionPointRightAttachment => "ejection_point_Right";
	public virtual string FireAnimLeftName => "fire_left";
	public virtual string FireAnimRightName => "fire_right";

	public bool IsSecondary;

	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		if ( !Silencer )
		{
			if ( !string.IsNullOrEmpty( MuzzleFlashParticle ) && EffectEntity.GetAttachment( IsSecondary ? MuzzleLeftAttachment : MuzzleRightAttachment ) != null )
				Particles.Create( MuzzleFlashParticle, EffectEntity, IsSecondary ? MuzzleLeftAttachment : MuzzleRightAttachment );

			if ( !string.IsNullOrEmpty( BulletEjectParticle ) && EffectEntity.GetAttachment( IsSecondary ? EjectionPointLeftAttachment : EjectionPointRightAttachment ) != null )
				Particles.Create( BulletEjectParticle, EffectEntity, IsSecondary ? EjectionPointLeftAttachment : EjectionPointRightAttachment );
		}

		if ( IsLocalPawn )
		{
			if ( ScreenShake != null )
				_ = new Sandbox.ScreenShake.Perlin( ScreenShake.Length, ScreenShake.Speed, ScreenShake.Size, ScreenShake.Rotation );
		}

		ViewModelEntity?.SetAnimBool( IsSecondary ? FireAnimLeftName : FireAnimRightName, true );
		CrosshairPanel?.CreateEvent( "fire" );

		IsSecondary = !IsSecondary;
	}

	[ClientRpc]
	protected override void EmptyEffects( bool isempty )
	{
		if ( isempty )
		{
			if ( AmmoClip > 1 ) return;

			ViewModelEntity?.SetAnimBool( "empty", true );
		}
		else
		{
			ViewModelEntity?.SetAnimBool( "empty", false );
		}
	}

	[ClientRpc]
	protected override void BulletTracer( Vector3 to )
	{
		var tr = EffectEntity.GetAttachment( IsSecondary ? MuzzleLeftAttachment : MuzzleRightAttachment );

		if ( tr == null ) return;

		var ps = Particles.Create( "particles/sd_bullet_trail.vpcf", to );
		ps.SetPosition( 0, tr.Value.Position );
		ps.SetPosition( 1, to );
	}

	public override void OnReloadFinish()
	{
		base.OnReloadFinish();

		IsSecondary = false;
	}
}
