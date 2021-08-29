using Sandbox;

[Library( "weapon_nicegun", Title = "Nice Gun", Spawnable = true )]
[Hammer.EditorModel("weapons/rust_pumpshotgun/rust_pumpshotgun.vmdl")]
partial class NiceGun : Weapon
{
	public override string ViewModelPath => "weapons/rust_pumpshotgun/v_rust_pumpshotgun.vmdl";

	public override int ClipSize => 2;
	public override float PrimaryRate => 15.0f;
	public override float SecondaryRate => 10.0f;
	public override float ReloadTime => 0.3f;
	public override int Bucket => 2;
	public override CType Crosshair => CType.ShotGun;
	public override string Icon => "ui/weapons/weapon_shotgun.png";

	public TimeSince TimeSinceDischarge { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/rust_pumpshotgun/rust_pumpshotgun.vmdl" );
	}

	public override bool CanPrimaryAttack()
	{
		return base.CanPrimaryAttack() && Input.Pressed( InputButton.Attack1 );
	}

	public override void AttackPrimary()
	{
		if ( !BaseAttackPrimary() ) return;

		PlaySound( "rust_pumpshotgun.shoot" );
		ShootBullets( 50, 0.3f, 10.0f, 9.0f, 3.0f );

		//SandboxPlayer ply = Owner as SandboxPlayer;

		// Error: Not Authority
		//if ( ply.Vehicle != null )
		//{
		//	ply.Vehicle.ApplyAbsoluteImpulse( Owner.EyeRot.Backward * 200.0f );
		//}

		Owner.ApplyAbsoluteImpulse( Owner.EyeRot.Backward * 500.0f );
	}

	public override void AttackSecondary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if ( !TakeAmmo( 2 ) )
		{
			if ( AmmoClip > 0 )
			{
				AttackPrimary();

				return;
			}
			else
			{
				DryFire();

				return;
			}
		}

		(Owner as AnimEntity)?.SetAnimBool( "b_attack", true );

		//
		// Tell the clients to play the shoot effects
		//
		DoubleShootEffects();
		PlaySound( "rust_pumpshotgun.shootdouble" );

		//
		// Shoot the bullets
		//
		ShootBullets( 100, 0.6f, 20.0f, 8.0f, 5.0f );

		Owner.ApplyAbsoluteImpulse( Owner.EyeRot.Backward * 5000.0f );
	}

	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );
		Particles.Create( "particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point" );

		ViewModelEntity?.SetAnimBool( "fire", true );

		if ( IsLocalPawn )
		{
			new Sandbox.ScreenShake.Perlin( 1.0f, 1.5f, 2.0f );
		}

		CrosshairPanel?.CreateEvent( "fire" );
	}

	[ClientRpc]
	protected virtual void DoubleShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );

		ViewModelEntity?.SetAnimBool( "fire_double", true );
		CrosshairPanel?.CreateEvent( "fire" );

		if ( IsLocalPawn )
		{
			new Sandbox.ScreenShake.Perlin( 3.0f, 3.0f, 3.0f );
		}
	}

	private void Discharge()
	{
		var muzzle = GetAttachment( "muzzle" ) ?? default;
		var pos = muzzle.Position;
		var rot = muzzle.Rotation;

		TakeAmmo( 1 );
		ShootEffects();
		PlaySound( "rust_pumpshotgun.shoot" );

		for ( int i = 0; i < 50; i++ )
			ShootBullet( pos, rot.Forward, 0.3f, 10.0f, 9.0f, 3.0f );

		ApplyAbsoluteImpulse( rot.Backward * 10000.0f );
	}

	protected override void OnPhysicsCollision( CollisionEventData eventData )
	{
		if ( eventData.Speed > 500.0f )
		{
			if ( AmmoClip > 0 )
				Discharge();
		}
	}

	public override void OnReloadFinish()
	{
		IsReloading = false;

		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if ( AmmoClip >= ClipSize )
			return;

		if ( Input.Down( InputButton.Attack1 ) || Input.Down( InputButton.Attack2 ) )
		{
			FinishReload();

			return;
		}

		AmmoClip += 1;

		if ( AmmoClip < ClipSize )
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

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 3 ); // TODO this is shit
		anim.SetParam( "aimat_weight", 1.0f );
	}
}
