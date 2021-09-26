using Sandbox;
using System.Collections.Generic;

[Library( "weapon_sg550", Title = "SG-550", Spawnable = true )]
[Hammer.EditorModel( "weapons/css_sg550/w_css_snip_sg550.vmdl" )]
partial class SG550 : WeaponSniper
{
	public override string ViewModelPath => "weapons/css_sg550/v_css_snip_sg550.vmdl";
	public override string WorldModelPath => "weapons/css_sg550/w_css_snip_sg550.vmdl";

	public override int ClipSize => 30;
	public override int Bucket => 0;
	public override int AmmoMultiplier => 5;
	public override float PrimaryRate => 5f;
	public override float ReloadTime => 3.75f;
	public override CType Crosshair => CType.None;
	public override string Icon => "ui/weapons/weapon_sg550.png";
	public override string ShootSound => "css_sg550.fire";
	public override float Spread => 0.4f;
	public override float ScopedSpread => 0.02f;
	public override float Force => 7f;
	public override float Damage => 60f;
	public override float BulletSize => 5f;
	public override float FOV => 75;
	public override ScreenShake ScreenShake => new ScreenShake
	{
		Length = 0.5f,
		Speed = 4.0f,
		Size = 1f,
		Rotation = 0.5f
	};
	public override List<float> ZoomLevels => new() { 30f, 15f };
	public override List<float> MouseSensitivity => new() { 0.3f, 0.2f };

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 2 ); // TODO this is shit
		anim.SetParam( "aimat_weight", 1.0f );
	}
}
