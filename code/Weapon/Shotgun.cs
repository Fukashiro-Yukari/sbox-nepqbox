using Sandbox;

[Spawnable]
[Library( "weapon_shotgun", Title = "Shotgun" )]
[EditorModel( "weapons/rust_shotgun/rust_shotgun.vmdl" )]
partial class Shotgun : Weapon
{ 
	public override string ViewModelPath => "weapons/rust_shotgun/v_rust_shotgun.vmdl";
	public override string WorldModelPath => "weapons/rust_shotgun/rust_shotgun.vmdl";
	public override int ClipSize => 1;
	public override float PrimaryRate => 1f;
	public override float SecondaryRate => 1f;
	public override float ReloadTime => 5f;
	public override int Bucket => 3;
	public override CType Crosshair => CType.ShotGun;
	public override bool RealReload => false;
	public override string Icon => "ui/weapons/weapon_shotgun.png";
	public override string ShootSound => "rust_pumpshotgun.shoot";
	public override int NumBullets => 15;
	public override float Spread => 0.5f;
	public override float Force => 10.0f;
	public override float Damage => 12.0f;
	public override float BulletSize => 3.0f;
	public override ScreenShake ScreenShake => new ScreenShake
	{
		Length = 1.0f,
		Delay = 1.5f,
		Size = 2.0f,
	};

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", 3 ); // TODO this is shit
		anim.SetAnimParameter( "aim_body_weight", 1.0f );
	}
}
