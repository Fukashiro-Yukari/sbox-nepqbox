using Sandbox;

[Library( "weapon_m249", Title = "M249", Spawnable = true )]
[Hammer.EditorModel( "weapons/css_m249/css_w_mach_m249para.vmdl" )]
partial class M249 : Weapon
{
	public override string ViewModelPath => "weapons/css_m249/css_v_mach_m249para.vmdl";
	public override string WorldModelPath => "weapons/css_m249/css_w_mach_m249para.vmdl";

	public override int ClipSize => 100;
	public override int Bucket => 0;
	public override float PrimaryRate => 12.5f;
	public override float ReloadTime => 5.7f;
	public override CType Crosshair => CType.Rifle;
	public override string Icon => "ui/weapons/weapon_m249.png";
	public override string ShootSound => "css_m249.fire";
	public override float Spread => 0.2f;
	public override float Force => 4f;
	public override float Damage => 15f;
	public override float BulletSize => 5f;
	public override float FOV => 75;
	public override ScreenShake ScreenShake => new ScreenShake
	{
		Length = 0.5f,
		Speed = 4.0f,
		Size = 1.0f,
		Rotation = 0.5f
	};

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 2 ); // TODO this is shit
		anim.SetParam( "aimat_weight", 1.0f );
	}
}
