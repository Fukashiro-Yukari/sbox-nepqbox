using Sandbox;

[Library( "weapon_famas", Title = "FAMAS", Spawnable = true )]
[Hammer.EditorModel( "weapons/css_famas/w_css_rif_famas.vmdl" )]
partial class FAMAS : Weapon
{
	public override string ViewModelPath => "weapons/css_famas/v_css_rif_famas.vmdl";
	public override string WorldModelPath => "weapons/css_famas/w_css_rif_famas.vmdl";

	public override int ClipSize => 25;
	public override int Bucket => 0;
	public override float PrimaryRate => 10f;
	public override float ReloadTime => 3.33f;
	public override CType Crosshair => CType.Rifle;
	public override string Icon => "ui/weapons/weapon_famas.png";
	public override string ShootSound => "css_famas.fire";
	public override bool UseBursts => true;
	public override float Spread => 0.15f;
	public override float Force => 3f;
	public override float Damage => 35f;
	public override float BulletSize => 4f;
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
