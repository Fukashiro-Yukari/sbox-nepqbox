using Sandbox;

[Spawnable]
[Library( "weapon_deagle", Title = "Desert Eagle" )]
[EditorModel( "weapons/css_deagle/css_w_pist_deagle.vmdl" )]
partial class Deagle : Weapon
{
	public override string ViewModelPath => "weapons/css_deagle/css_v_pist_deagle.vmdl";
	public override string WorldModelPath => "weapons/css_deagle/css_w_pist_deagle.vmdl";

	public override int ClipSize => 9;
	public override int Bucket => 1;
	public override int AmmoMultiplier => 5;
	public override bool Automatic => false;
	public override float PrimaryRate => 4f;
	public override float ReloadTime => 2.17f;
	public override CType Crosshair => CType.Pistol;
	public override string Icon => "ui/weapons/weapon_deagle.png";
	public override string ShootSound => "css_deagle.fire";
	public override float Spread => 0.06f;
	public override float Force => 5f;
	public override float Damage => 50f;
	public override float BulletSize => 6f;
	public override float FOV => 75;
	public override ScreenShake ScreenShake => new ScreenShake
	{
		Length = 0.5f,
		Delay = 4.0f,
		Size = 1.0f,
		Rotation = 0.5f
	};

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", 1 ); // TODO this is shit
		anim.SetAnimParameter( "holdtype_handedness", 0 );
		anim.SetAnimParameter( "aim_body_weight", 1.0f );
	}
}
