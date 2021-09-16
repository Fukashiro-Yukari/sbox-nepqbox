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
			if ( !string.IsNullOrEmpty( MuzzleFlashParticle ) )
				Particles.Create( MuzzleFlashParticle, EffectEntity, IsSecondary ? MuzzleLeftAttachment : MuzzleRightAttachment );

			if ( !string.IsNullOrEmpty( BulletEjectParticle ) )
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

	public override void OnReloadFinish()
	{
		base.OnReloadFinish();

		IsSecondary = false;
	}
}
