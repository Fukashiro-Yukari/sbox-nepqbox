using Sandbox;
using System;

[Spawnable]
[Library( "weapon_smg", Title = "SMG" )]
[Hammer.EditorModel( "weapons/rust_smg/rust_smg.vmdl" )]
partial class SMG : Weapon
{ 
	public override string ViewModelPath => "weapons/rust_smg/v_rust_smg.vmdl";
	public override string WorldModelPath => "weapons/rust_smg/rust_smg.vmdl";

	public override int ClipSize => 30;
	public override float PrimaryRate => 15.0f;
	public override float SecondaryRate => 1.0f;
	public override float ReloadTime => 4.0f;
	public override int Bucket => 2;
	public override CType Crosshair => CType.SMG;
	public override string Icon => "ui/weapons/weapon_smg.png";
	public override string ShootSound => "rust_smg.shoot";
	public override float Spread => 0.1f;
	public override float Force => 1.5f;
	public override float Damage => 5.0f;
	public override float BulletSize => 3.0f;
	public override ScreenShake ScreenShake => new ScreenShake
	{
		Length = 0.5f,
		Delay = 4.0f,
		Size = 1.0f,
		Rotation = 0.5f
	};

	public override void AttackSecondary()
	{
		// Grenade lob
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", 2 ); // TODO this is shit
		anim.SetAnimParameter( "aim_body_weight", 1.0f );
	}
}
