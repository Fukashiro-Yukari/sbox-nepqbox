using Sandbox;

public partial class WeaponShotgun : Weapon
{
	public override bool RealReload => false;

	TimeSince timeSinceStop;

	public override void Simulate( Client owner )
	{
		base.Simulate( owner );

		if ( IsReloading )
			if ( Input.Down( InputButton.PrimaryAttack ) || Input.Down( InputButton.SecondaryAttack ) )
				timeSinceStop = 0;
	}

	public override void OnReloadFinish()
	{
		IsReloading = false;

		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if ( AmmoClip >= ClipSize || AmmoCount <= 0 )
			return;

		if ( timeSinceStop < ReloadTime )
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
		ViewModelEntity?.SetAnimParameter( "reload_finished", true );
	}

	[ClientRpc]
	protected override void EmptyEffects( bool isempty )
	{
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", 3 ); // TODO this is shit
		anim.SetAnimParameter( "aim_body_weight", 1.0f );
	}
}
