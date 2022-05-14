using Sandbox;

[Library( "weapon_ump45", Title = "UMP 45", Spawnable = true )]
[Hammer.EditorModel( "weapons/css_ump45/w_css_smg_ump45.vmdl" )]
partial class UMP45 : Weapon
{
	public override string ViewModelPath => "weapons/css_ump45/v_css_smg_ump45.vmdl";
	public override string WorldModelPath => "weapons/css_ump45/w_css_smg_ump45.vmdl";

	public override int ClipSize => 25;
	public override int Bucket => 2;
	public override float PrimaryRate => 8.5f;
	public override float ReloadTime => 3.45f;
	public override CType Crosshair => CType.SMG;
	public override string Icon => "ui/weapons/weapon_ump45.png";
	public override string ShootSound => "css_ump45.fire";
	public override string MuzzleFlashParticle => null;
	public override float Spread => 0.18f;
	public override float Force => 3f;
	public override float Damage => 35f;
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
