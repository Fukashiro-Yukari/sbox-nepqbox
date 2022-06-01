using Sandbox;

[Spawnable]
[Library( "weapon_mp5navy", Title = "MP5" )]
[Hammer.EditorModel( "weapons/css_mp5/w_css_smg_mp5.vmdl" )]
partial class MP5 : Weapon
{
	public override string ViewModelPath => "weapons/css_mp5/v_css_smg_mp5.vmdl";
	public override string WorldModelPath => "weapons/css_mp5/w_css_smg_mp5.vmdl";

	public override int ClipSize => 32;
	public override int Bucket => 2;
	public override float PrimaryRate => 12.5f;
	public override float ReloadTime => 3.05f;
	public override CType Crosshair => CType.SMG;
	public override string Icon => "ui/weapons/weapon_mp5navy.png";
	public override string ShootSound => "css_mp5.fire";
	public override float Spread => 0.25f;
	public override float Force => 3f;
	public override float Damage => 30f;
	public override float BulletSize => 2f;
	public override float FOV => 75;
	public override ScreenShake ScreenShake => new ScreenShake
	{
		Length = 0.5f,
		Delay = 4.0f,
		Size = 0.5f,
		Rotation = 0.5f
	};

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", 2 ); // TODO this is shit
		anim.SetAnimParameter( "aim_body_weight", 1.0f );
	}
}
