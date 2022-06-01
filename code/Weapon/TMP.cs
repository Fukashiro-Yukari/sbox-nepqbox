using Sandbox;

[Spawnable]
[Library( "weapon_tmp", Title = "TMP" )]
[Hammer.EditorModel( "weapons/css_tmp/w_css_smg_tmp.vmdl" )]
partial class TMP : Weapon
{
	public override string ViewModelPath => "weapons/css_tmp/v_css_smg_tmp.vmdl";
	public override string WorldModelPath => "weapons/css_tmp/w_css_smg_tmp.vmdl";

	public override int ClipSize => 30;
	public override int Bucket => 2;
	public override float PrimaryRate => 12.5f;
	public override float ReloadTime => 2.12f;
	public override CType Crosshair => CType.SMG;
	public override string Icon => "ui/weapons/weapon_tmp.png";
	public override string ShootSound => "css_tmp.fire";
	public override string MuzzleFlashParticle => null;
	public override float Spread => 0.2f;
	public override float Force => 3f;
	public override float Damage => 30f;
	public override float BulletSize => 2f;
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
		anim.SetAnimParameter( "holdtype", 2 ); // TODO this is shit
		anim.SetAnimParameter( "aim_body_weight", 1.0f );
	}
}
