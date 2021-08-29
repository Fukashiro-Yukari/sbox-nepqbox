using Sandbox;


[Library( "weapon_shotgun", Title = "Shotgun", Spawnable = true )]
[Hammer.EditorModel( "weapons/rust_shotgun/rust_shotgun.vmdl" )]
partial class Shotgun : Weapon
{ 
	public override string ViewModelPath => "weapons/rust_shotgun/v_rust_shotgun.vmdl";
	public override int ClipSize => 1;
	public override float PrimaryRate => 1f;
	public override float SecondaryRate => 1f;
	public override float ReloadTime => 5f;
	public override int Bucket => 2;
	public override CType Crosshair => CType.ShotGun;
	public override bool RealReload => false;
	public override string Icon => "ui/weapons/weapon_shotgun.png";

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/rust_shotgun/rust_shotgun.vmdl" );
	}

	public override void AttackPrimary()
	{
		if ( !BaseAttackPrimary() ) return;

		PlaySound("rust_pumpshotgun.shoot");

		//
		// Shoot the bullets
		//
		ShootBullets( 15, 0.5f, 10.0f, 12.0f, 3.0f );
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
			new Sandbox.ScreenShake.Perlin(1.0f, 1.5f, 2.0f);
		}

		CrosshairPanel?.CreateEvent( "fire" );
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 3 ); // TODO this is shit
		anim.SetParam( "aimat_weight", 1.0f );
	}
}
