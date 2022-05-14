using Sandbox;

[Library( "crossbow_bolt" )]
[Hammer.Skip]
partial class CrossbowBolt : ModelEntity
{
	bool Stuck;

	public virtual string ModelPath => "weapons/rust_crossbow/rust_crossbow_bolt.vmdl";
	public virtual string Icon => "ui/weapons/weapon_crossbow.png";

	public override void Spawn()
	{
		base.Spawn();

		SetModel(ModelPath);
	}

	[Event.Tick.Server]
	public virtual void Tick()
	{
		if ( !IsServer )
			return;

		if (Stuck)
			return;

		float Speed = 10000.0f;
		var velocity = Rotation.Forward * Speed;

		var start = Position;
		var end = start + velocity * Time.Delta;

		var tr = Trace.Ray( start, end )
				.UseHitboxes()
				//.HitLayer( CollisionLayer.Water, !InWater )
				.Ignore( Owner )
				.Ignore( this )
				.Size( 1.0f )
				.Run();


		if ( tr.Hit )
		{
			// TODO: CLINK NOISE (unless flesh)

			// TODO: SPARKY PARTICLES (unless flesh)

			Stuck = true;
			Position = tr.EndPosition + Rotation.Forward * -1;

			if ( tr.Entity.IsValid() )
			{
				var damageInfo = DamageInfo.FromBullet( tr.EndPosition, tr.Direction * 200, 60.0f )
													.UsingTraceResult( tr )
													.WithAttacker( Owner )
													.WithWeapon( this );

				tr.Entity.TakeDamage( damageInfo );
            }

			var npc = tr.Entity as NPC;
			var prop = tr.Entity as Prop;

			// TODO: Parent to bone so this will stick in the meaty heads
			if (npc != null && npc.Corpse != null)
				SetParent(npc?.Corpse, tr.Bone);
            else if (prop != null)
            {
				if (prop.GetModel().GetPropData().Health > 0)
                {
					if (prop.Health > 0)
						SetParent(prop, tr.Bone);
                    else
						Delete();
				}
				else if (prop.GetModel().GetPropData().Health <= 0)
					SetParent(prop, tr.Bone);
			}
			else if (tr.Entity is WorldEntity || tr.Entity.Health > 0)
				SetParent(tr.Entity, tr.Bone);

			Owner = null;

			//
			// Surface impact effect
			//
			tr.Normal = Rotation.Forward * -1;
			tr.Surface.DoBulletImpact( tr );
			velocity = default;

			// delete self in 60 seconds
			_ = DeleteAsync( 60.0f );
		}
		else
		{
			Position = end;
		}
	}
}
