using Sandbox;

[Library( "weapon_galil", Title = "Galil", Spawnable = true )]
[Hammer.EditorModel( "weapons/css_galil/w_css_rif_galil.vmdl" )]
partial class GALIL : Weapon
{
	public override string ViewModelPath => "weapons/css_galil/v_css_rif_galil.vmdl";
	public override string WorldModelPath => "weapons/css_galil/w_css_rif_galil.vmdl";

	public override int ClipSize => 35;
	public override int Bucket => 0;
	public override float PrimaryRate => 8.3f;
	public override float ReloadTime => 2.95f;
	public override CType Crosshair => CType.Rifle;
	public override string Icon => "ui/weapons/weapon_galil.png";
	public override string ShootSound => "css_galil.fire";
	public override float Spread => 0.18f;
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
