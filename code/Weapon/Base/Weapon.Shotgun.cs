using Sandbox;

public partial class WeaponShotgun : Weapon
{
	public override bool RealReload => false;

	public override void OnReloadFinish()
	{
		IsReloading = false;

		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if ( AmmoClip >= ClipSize || AmmoCount <= 0 )
			return;

		if ( Input.Down( InputButton.Attack1 ) || Input.Down( InputButton.Attack2 ) )
		{
			FinishReload();

			return;
		}

		AmmoClip += TakeAmmo2( 1 );

		if ( AmmoClip < ClipSize && AmmoCount > 0 )
		{
			Reload();
		}
		else
		{
			FinishReload();
		}
	}

	[ClientRpc]
	protected virtual void FinishReload()
	{
		ViewModelEntity?.SetAnimBool( "reload_finished", true );
	}

	[ClientRpc]
	protected override void EmptyEffects( bool isempty )
	{
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 3 ); // TODO this is shit
		anim.SetParam( "aimat_weight", 1.0f );
	}
}
