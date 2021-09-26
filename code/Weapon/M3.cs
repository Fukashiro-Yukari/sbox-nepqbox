using Sandbox;

[Library( "weapon_m3", Title = "M3 Super 90", Spawnable = true )]
[Hammer.EditorModel( "weapons/css_super90/css_w_shot_m3super90.vmdl" )]
partial class M3 : WeaponShotgun
{
	public override string ViewModelPath => "weapons/css_super90/css_v_shot_m3super90.vmdl";
	public override string WorldModelPath => "weapons/css_super90/css_w_shot_m3super90.vmdl";

	public override int ClipSize => 8;
	public override int Bucket => 0;
	public override float PrimaryRate => 1f;
	public override float ReloadTime => 0.49f;
	public override CType Crosshair => CType.ShotGun;
	public override string Icon => "ui/weapons/weapon_m3.png";
	public override string ShootSound => "css_super90.fire";
	public override int NumBullets => 8;
	public override float Spread => 0.3f;
	public override float Force => 5f;
	public override float Damage => 15f;
	public override float BulletSize => 2f;
	public override float FOV => 75;
	public override ScreenShake ScreenShake => new ScreenShake
	{
		Length = 0.5f,
		Speed = 4.0f,
		Size = 1.0f,
		Rotation = 0.5f
	};
}
