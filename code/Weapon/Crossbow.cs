using Sandbox;
using System;

[Spawnable]
[Library( "weapon_crossbow", Title = "Crossbow" )]
[Hammer.EditorModel( "weapons/rust_crossbow/rust_crossbow.vmdl" )]
partial class Crossbow : WeaponSniper
{ 
	public override string ViewModelPath => "weapons/rust_crossbow/v_rust_crossbow.vmdl";
	public override string WorldModelPath => "weapons/rust_crossbow/rust_crossbow.vmdl";

	public override int ClipSize => 1;
	public override float PrimaryRate => 1;
	public override int Bucket => 3;
    public override string ReloadSound => "rush_crossbow.reload";
	public override string ShootSound => "rush_crossbow.shoot";
	public override string Icon => "ui/weapons/weapon_crossbow.png";
	public override string BulletEjectParticle => "";
	public override string MuzzleFlashParticle => "";
	public override ScreenShake ScreenShake => new ScreenShake
	{
		Length = 0.5f,
		Delay = 4.0f,
		Size = 1.0f,
		Rotation = 0.5f
	};
	public override CType Crosshair => CType.Crossbow;

	public override Func<Vector3, Vector3, Vector3, float, float, float, Entity> CreateEntity => CreateCrossbow;

	private Entity CreateCrossbow( Vector3 pos, Vector3 dir, Vector3 forward, float spread, float force, float damage )
	{
		if ( IsClient ) return null;
		using ( Prediction.Off() )
		{
			var bolt = new CrossbowBolt();
			bolt.Position = pos;
			bolt.Rotation = Owner.EyeRotation;
			bolt.Owner = Owner;
			bolt.Velocity = Owner.EyeRotation.Forward * 100;

			return bolt;
		}
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", 3 ); // TODO this is shit
		anim.SetAnimParameter( "aim_body_weight", 1.0f );
	}

	//[Event.Tick]
	//void Tick()
	//{
		//Log.Info((IsServer ? "Server " : "Client ") + (AmmoClip >= 1));

		//ViewModelEntity?.SetAnimParameter( "loaded", false );
	//}
}
