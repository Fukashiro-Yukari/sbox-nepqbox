using Sandbox;
using System;

[Library("weapon_smgshotgun", Title = "SMG Like Shotgun", Spawnable = true )]
[Hammer.EditorModel( "weapons/rust_smg/rust_smg.vmdl" )]
partial class SMGShotgun : Weapon
{ 
	public override string ViewModelPath => "weapons/rust_smg/v_rust_smg.vmdl";
	public override string WorldModelPath => "weapons/rust_smg/rust_smg.vmdl";

	public override int ClipSize => 30;
	public override float PrimaryRate => 8.0f;
	public override float SecondaryRate => 1.0f;
	public override float ReloadTime => 4.0f;
	public override int Bucket => 2;
	public override CType Crosshair => CType.ShotGun;
	public override string Icon => "ui/weapons/weapon_smg.png";
	public override string ShootSound => "rust_pumpshotgun.shoot";
	public override int NumBullets => 6;
	public override float Spread => 0.3f;
	public override float Force => 10.0f;
	public override float Damage => 6.0f;
	public override float BulletSize => 3.0f;
	public override ScreenShake ScreenShake => new ScreenShake
	{
		Length = 0.5f,
		Speed = 4.0f,
		Size = 1.0f,
		Rotation = 0.5f
	};

	public override void AttackSecondary()
	{
		// Grenade lob
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 2 ); // TODO this is shit
		anim.SetParam( "aimat_weight", 1.0f );
	}
}
