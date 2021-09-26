using Sandbox;
using System.Collections.Generic;

[Library( "weapon_g3sg1", Title = "G3/SG-1", Spawnable = true )]
[Hammer.EditorModel( "weapons/css_g3sg1/w_css_snip_g3sg1.vmdl" )]
partial class G3SG1 : WeaponSniper
{
	public override string ViewModelPath => "weapons/css_g3sg1/v_css_snip_g3sg1.vmdl";
	public override string WorldModelPath => "weapons/css_g3sg1/w_css_snip_g3sg1.vmdl";

	public override int ClipSize => 20;
	public override int Bucket => 0;
	public override int AmmoMultiplier => 5;
	public override float PrimaryRate => 4f;
	public override float ReloadTime => 4.67f;
	public override CType Crosshair => CType.None;
	public override string Icon => "ui/weapons/weapon_g3sg1.png";
	public override string ShootSound => "css_g3sg1.fire";
	public override float Spread => 0.5f;
	public override float ScopedSpread => 0.03f;
	public override float Force => 7f;
	public override float Damage => 70f;
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
