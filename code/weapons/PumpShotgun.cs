using Sandbox;

[Library( "weapon_pumpshotgun", Title = "Pump Shotgun", Spawnable = true )]
[Hammer.EditorModel( "weapons/rust_pumpshotgun/rust_pumpshotgun.vmdl" )]
partial class PumpShotgun : WeaponShotgun
{ 
	public override string ViewModelPath => "weapons/rust_pumpshotgun/v_rust_pumpshotgun.vmdl";
	public override string WorldModelPath => "weapons/rust_pumpshotgun/rust_pumpshotgun.vmdl";

	public override int ClipSize => 8;
	public override int Bucket => 2;
	public override float PrimaryRate => 1f;
	public override float SecondaryRate => 1f;
	public override float ReloadTime => 0.4f;
	public override CType Crosshair => CType.ShotGun;
	public override string Icon => "ui/weapons/weapon_shotgun.png";
	public override string ShootSound => "rust_pumpshotgun.shoot";
	public override int NumBullets => 10;
	public override float Spread => 0.1f;
	public override float Force => 10.0f;
	public override float Damage => 9.0f;
	public override float BulletSize => 3.0f;
	public override ScreenShake ScreenShake => new ScreenShake
	{
		Length = 1.0f,
		Speed = 1.5f,
		Size = 2.0f,
	};

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

		(Owner as AnimEntity).SetAnimBool( "b_attack", true );

		//
		// Tell the clients to play the shoot effects
		//
		DoubleShootEffects();
		PlaySound( "rust_pumpshotgun.shootdouble" );

		//
		// Shoot the bullets
		//
		ShootBullets( 20, 0.4f, 20.0f, 8.0f, 3.0f );
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
			new Sandbox.ScreenShake.Perlin(3.0f, 3.0f, 3.0f);
		}
	}
}
