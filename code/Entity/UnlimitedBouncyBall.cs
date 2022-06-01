using Sandbox;
using System;

[Spawnable]
[Library( "ent_unlimited_bouncyball", Title = "Unlimited Bouncy Ball" )]
public partial class UnlimitedBouncyBallEntity : Prop, IUse
{
	public float SpeedMul { get; set; } = 1.2f;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/ball/ball.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
		Scale = Rand.Float( 0.5f, 2.0f );
		RenderColor = Color.Random;
	}

	protected override void OnPhysicsCollision( CollisionEventData eventData )
	{
		var speed = eventData.PreVelocity.Length;
		var direction = Vector3.Reflect( eventData.PreVelocity.Normal, eventData.Normal.Normal ).Normal;

		var propData = GetModelPropData();

		var minImpactSpeed = propData.MinImpactDamageSpeed;
		if ( minImpactSpeed <= 0.0f ) minImpactSpeed = 100;

		var impactDmg = propData.ImpactDamage;
		if ( impactDmg <= 0.0f ) impactDmg = 10;

		if ( speed > minImpactSpeed )
		{
			if ( eventData.Entity.IsValid() && eventData.Entity != this )
			{
				var damage = speed / minImpactSpeed * impactDmg * 1.2f;
				eventData.Entity.TakeDamage( DamageInfo.Generic( damage )
					.WithFlag( DamageFlags.PhysicsImpact )
					.WithAttacker( this )
					.WithPosition( eventData.Position )
					.WithForce( eventData.PreVelocity ) );
			}
		}

		Velocity = direction * speed * SpeedMul;
	}

	public bool IsUsable( Entity user )
	{
		return true;
	}

	public bool OnUse( Entity user )
	{
		if ( user is Player player )
		{
			player.Health += 10;

			Delete();
		}

		return false;
	}
}
