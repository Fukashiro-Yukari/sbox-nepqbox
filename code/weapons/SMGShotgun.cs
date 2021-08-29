using Sandbox;
using System;

[Library("weapon_smgshotgun", Title = "SMG Like Shotgun", Spawnable = true )]
[Hammer.EditorModel( "weapons/rust_smg/rust_smg.vmdl" )]
partial class SMGShotgun : Weapon
{ 
	public override string ViewModelPath => "weapons/rust_smg/v_rust_smg.vmdl";

	public override int ClipSize => 30;
	public override float PrimaryRate => 8.0f;
	public override float SecondaryRate => 1.0f;
	public override float ReloadTime => 4.0f;
	public override int Bucket => 2;
	public override CType Crosshair => CType.ShotGun;
	public override string Icon => "ui/weapons/weapon_smg.png";

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/rust_smg/rust_smg.vmdl" );
	}

	public override void AttackPrimary()
	{
		if ( !BaseAttackPrimary() ) return;

		PlaySound("rust_pumpshotgun.shoot");

		//
		// Shoot the bullets
		//
		ShootBullets(6, 0.3f, 10.0f, 6.0f, 3.0f);
	}

	public override void AttackSecondary()
	{
		// Grenade lob
	}

	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );
		Particles.Create( "particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point" );

		if ( Owner == Local.Pawn )
		{
			new Sandbox.ScreenShake.Perlin(0.5f, 4.0f, 1.0f, 0.5f);
		}

		ViewModelEntity?.SetAnimBool( "fire", true );
		CrosshairPanel?.CreateEvent( "fire" );
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 2 ); // TODO this is shit
		anim.SetParam( "aimat_weight", 1.0f );
	}
}
