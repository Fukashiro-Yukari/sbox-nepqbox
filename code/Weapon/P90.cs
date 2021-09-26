using Sandbox;
using System.Collections.Generic;

[Library( "weapon_p90", Title = "P90", Spawnable = true )]
[Hammer.EditorModel( "weapons/css_p90/w_css_smg_p90.vmdl" )]
partial class P90 : WeaponSniper
{
	public override string ViewModelPath => "weapons/css_p90/v_css_smg_p90.vmdl";
	public override string WorldModelPath => "weapons/css_p90/w_css_smg_p90.vmdl";

	public override int ClipSize => 50;
	public override int Bucket => 0;
	public override float PrimaryRate => 12.5f;
	public override float ReloadTime => 3.38f;
	public override CType Crosshair => CType.SMG;
	public override string Icon => "ui/weapons/weapon_p90.png";
	public override string ShootSound => "css_p90.fire";
	public override float Spread => 0.15f;
	public override float ScopedSpread => 0.015f;
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
	public override List<float> ZoomLevels => new() { 40f };
	public override List<float> MouseSensitivity => new() { 0.4f };

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 2 ); // TODO this is shit
		anim.SetParam( "aimat_weight", 1.0f );
	}
}
