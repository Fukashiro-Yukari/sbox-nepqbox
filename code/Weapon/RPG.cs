using Sandbox;
using System;
using System.Collections.Generic;

[Library( "weapon_rpg", Title = "RPG-7", Spawnable = true )]
[Hammer.EditorModel( "weapons/swb/explosives/rpg-7/swb_w_rpg7.vmdl" )]
partial class RPG : Weapon
{
	public override string ViewModelPath => "weapons/swb/explosives/rpg-7/swb_v_rpg7.vmdl";
	public override string WorldModelPath => "weapons/swb/explosives/rpg-7/swb_w_rpg7.vmdl";

	public override int ClipSize => 1;
	public override int Bucket => 0;
	public override int AmmoMultiplier => 10;
	public override bool RealReload => false;
	public override float PrimaryRate => 1f;
	public override float ReloadTime => 4f;
	public override CType Crosshair => CType.Rifle;
	public override string Icon => "ui/weapons/weapon_ak47.png";
	public override string ShootSound => "swb_explosives_rpg7.fire";
	public override string MuzzleFlashParticle => "particles/swb/smoke/swb_smokepuff_1.vpcf";
	public override string DrawEmptyAnim => "deploy_empty";
	public override float Spread => 0.2f;
	public override float Force => 4f;
	public override float Damage => 15f;
	public override float BulletSize => 5f;
	public override float FOV => 40;
	public override ScreenShake ScreenShake => new ScreenShake
	{
		Length = 0.5f,
		Speed = 4.0f,
		Size = 0.5f,
		Rotation = 0.5f
	};
	public override Func<Vector3, Vector3, Vector3, float, float, float, Entity> CreateEntity => CreateRocketEntity;

	private Entity CreateRocketEntity( Vector3 pos, Vector3 dir, Vector3 forward, float spread, float force, float damage )
	{
		if ( IsClient ) return null;

		using ( Prediction.Off() )
		{
			var rocket = new Rocket();
			rocket.Weapon = this;
			rocket.ExplosionDelay = 3f;
			rocket.ExplosionRadius = 400f;
			rocket.ExplosionDamage = 300f;
			rocket.ExplosionForce = 500f;
			rocket.ExplosionSounds = new List<string>
		{
			"swb_explosion_1",
			"swb_explosion_2",
			"swb_explosion_3",
			"swb_explosion_4",
			"swb_explosion_5"
		};
			rocket.ExplosionEffect = "weapons/css_grenade_he/particles/grenade_he_explosion.vpcf";
			rocket.RocketSound = "swb_explosives_rpg7.rocketloop";
			rocket.RocketEffects = new List<string>
		{
			"particles/swb/smoke/swb_smoketrail_1.vpcf",
			"particles/swb/fire/swb_fire_rocket_1.vpcf"
		};
			rocket.ExplosionShake = new ScreenShake
			{
				Length = 1f,
				Speed = 5f,
				Size = 7f,
				Rotation = 3f
			};

			rocket.Owner = Owner;
			rocket.Position = MathUtil.RelativeAdd( Position, new Vector3( 10, 10, 10 ), Owner.EyeRot );
			rocket.Rotation = Owner.EyeRot * Rotation.From( new Angles( 0, 180, 0 ) );
			rocket.Speed = 30;
			rocket.StartVelocity = MathUtil.RelativeAdd( Vector3.Zero, new Vector3( 0, 0, 3000 ), Owner.EyeRot );
			rocket.Start();

			return rocket;
		}
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 2 ); // TODO this is shit
		anim.SetParam( "aimat_weight", 1.0f );
	}
}
