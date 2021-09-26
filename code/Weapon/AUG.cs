using Sandbox;
using System.Collections.Generic;

[Library( "weapon_aug", Title = "AUG", Spawnable = true )]
[Hammer.EditorModel( "weapons/css_ang/w_css_rif_aug.vmdl" )]
partial class AUG : WeaponSniper
{
	public override string ViewModelPath => "weapons/css_ang/v_css_rif_aug.vmdl";
	public override string WorldModelPath => "weapons/css_ang/w_css_rif_aug.vmdl";

	public override int ClipSize => 30;
	public override int Bucket => 0;
	public override float PrimaryRate => 10f;
	public override float ReloadTime => 3.77f;
	public override CType Crosshair => CType.Rifle;
	public override string Icon => "ui/weapons/weapon_aug.png";
	public override string ShootSound => "css_aug.fire";
	public override float Spread => 0.18f;
	public override float ScopedSpread => 0.08f;
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
