using Sandbox;

[Library( "weapon_xm1014", Title = "XM1014", Spawnable = true )]
[Hammer.EditorModel( "weapons/css_xm1014/w_css_shot_xm1014.vmdl" )]
partial class XM1014 : WeaponShotgun
{
	public override string ViewModelPath => "weapons/css_xm1014/v_css_shot_xm1014.vmdl";
	public override string WorldModelPath => "weapons/css_xm1014/w_css_shot_xm1014.vmdl";

	public override int ClipSize => 7;
	public override int Bucket => 0;
	public override bool Automatic => false;
	public override float PrimaryRate => 5f;
	public override float ReloadTime => 0.39f;
	public override CType Crosshair => CType.ShotGun;
	public override string Icon => "ui/weapons/weapon_xm1014.png";
	public override string ShootSound => "css_xm1014.fire";
	public override int NumBullets => 6;
	public override float Spread => 0.8f;
	public override float Force => 5f;
	public override float Damage => 10f;
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
