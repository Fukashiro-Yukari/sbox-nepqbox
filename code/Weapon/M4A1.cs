using Sandbox;

[Library( "weapon_m4a1", Title = "M4A1", Spawnable = true )]
[Hammer.EditorModel( "weapons/css_m4a1/css_w_m4a1.vmdl" )]
partial class M4A1 : Weapon
{
	public override string ViewModelPath => "weapons/css_m4a1/css_v_m4a1.vmdl";
	public override string WorldModelPath => "weapons/css_m4a1/css_w_m4a1.vmdl";
	public override string SilencerWorldModelPath => "weapons/css_m4a1/css_w_m4a1_silencer.vmdl";

	public override int ClipSize => 30;
	public override int Bucket => 0;
	public override bool UseSilencer => true;
	public override float PrimaryRate => 10f;
	public override float ReloadTime => 3.05f;
	public override CType Crosshair => CType.Rifle;
	public override string Icon => "ui/weapons/weapon_m4a1.png";
	public override string ShootSound => "css_m4a1.fire";
	public override string SilencerShootSound => "css_m4a1.fire_silenced";
	public override float Spread => 0.08f;
	public override float Force => 2.5f;
	public override float Damage => 13f;
	public override float BulletSize => 3.5f;
	public override float FOV => 75;
	public override ScreenShake ScreenShake => new ScreenShake
	{
		Length = 0.5f,
		Speed = 4.0f,
		Size = 0.5f,
		Rotation = 0.5f
	};

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 2 ); // TODO this is shit
		anim.SetParam( "aimat_weight", 1.0f );
	}
}
