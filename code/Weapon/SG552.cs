using Sandbox;
using System.Collections.Generic;

[Library( "weapon_sg552", Title = "SG 552", Spawnable = true )]
[Hammer.EditorModel( "weapons/css_sg552/w_css_rif_sg552.vmdl" )]
partial class SG552 : WeaponSniper
{
	public override string ViewModelPath => "weapons/css_sg552/v_css_rif_sg552.vmdl";
	public override string WorldModelPath => "weapons/css_sg552/w_css_rif_sg552.vmdl";

	public override int ClipSize => 30;
	public override int Bucket => 0;
	public override float PrimaryRate => 10f;
	public override float ReloadTime => 2.76f;
	public override CType Crosshair => CType.Rifle;
	public override string Icon => "ui/weapons/weapon_sg552.png";
	public override string ShootSound => "css_sg552.fire";
	public override float Spread => 0.16f;
	public override float ScopedSpread => 0.06f;
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
	public override List<float> ZoomLevels => new() { 30f };
	public override List<float> MouseSensitivity => new() { 0.4f };

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 2 ); // TODO this is shit
		anim.SetParam( "aimat_weight", 1.0f );
	}
}
