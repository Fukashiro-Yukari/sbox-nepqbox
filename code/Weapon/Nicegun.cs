using Sandbox;
using Utils

[Spawnable]
[Library( "weapon_nicegun", Title = "Nice Gun" )]
[Hammer.EditorModel("weapons/rust_pumpshotgun/rust_pumpshotgun.vmdl")]
partial class NiceGun : WeaponShotgun
{
	public override string ViewModelPath => "weapons/rust_pumpshotgun/v_rust_pumpshotgun.vmdl";
	public override string WorldModelPath => "weapons/rust_pumpshotgun/rust_pumpshotgun.vmdl";

	public override int ClipSize => 2;
	public override int Bucket => 3;
	public override float PrimaryRate => 15.0f;
	public override float SecondaryRate => 10.0f;
	public override float ReloadTime => 0.3f;
	public override bool Automatic => false;
	public override CType Crosshair => CType.ShotGun;
	public override string Icon => "ui/weapons/weapon_shotgun.png";
	public override string ShootSound => "rust_pumpshotgun.shoot";
	public override bool CanDischarge => true;
	public override int NumBullets => 50;
	public override float Spread => 0.3f;
	public override float Force => 10f;
	public override float Damage => 9.0f;
	public override float BulletSize => 3.0f;
	public override float DischargeRecoil => 10000.0f;
	public override ScreenShake ScreenShake => new ScreenShake
	{
		Length = 1.0f,
		Delay = 1.5f,
		Size = 2.0f,
	};

    public override void AttackPrimary()
	{
		base.AttackPrimary();

		SandboxPlayer ply = Owner as SandboxPlayer;

		if (IsClient) return;
		//if (ply.Vehicle != null)
		//	ply.Vehicle.ApplyAbsoluteImpulse(Owner.EyeRotation.Backward * 500.0f);

        Owner.ApplyAbsoluteImpulse( Owner.EyeRotation.Backward * 500.0f );
	}

	public override void AttackSecondary()
	{
		TimeSincePrimaryAttack = -0.5f;
		TimeSinceSecondaryAttack = -0.5f;

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

		(Owner as AnimEntity)?.SetAnimParameter( "b_attack", true );

		//
		// Tell the clients to play the shoot effects
		//
		DoubleShootEffects();
		PlaySound( "rust_pumpshotgun.shootdouble" );

		//
		// Shoot the bullets
		//
		ShootBullets( 100, 0.6f, 20.0f, 8.0f, 5.0f );

		SandboxPlayer ply = Owner as SandboxPlayer;

		if (IsClient) return;
		//if (ply.Vehicle != null)
		//	ply.Vehicle.ApplyAbsoluteImpulse(Owner.EyeRotation.Backward * 5000.0f);

		Owner.ApplyAbsoluteImpulse( Owner.EyeRotation.Backward * 5000.0f );
	}

	[ClientRpc]
	protected virtual void DoubleShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );
		Particles.Create( "particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point" );

		ViewModelEntity?.SetAnimParameter( "fire_double", true );
		CrosshairShoot();

		if ( IsLocalPawn )
		{
			ScreenUtil.Shake( 3.0f, 2.0f, 3.0f, 3.0f );
		}
	}
}
