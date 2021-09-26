using Sandbox;

public class FiredEntity : ModelEntity
{
	public Weapon Weapon { get; set; } // The parent weapon
	public Vector3 StartVelocity { get; set; }
	public float RemoveDelay { get; set; }
	public float Damage { get; set; }
	public float Force { get; set; }
	public float Speed { get; set; }
	public bool UseGravity { get; set; }
	public bool IsSticky { get; set; }

	public bool canThink;
	public bool isStuck;

	public override void Spawn()
	{
		base.Spawn();
	}

	public virtual void Start()
	{
		// Initialize physics
		MoveType = MoveType.Physics;
		PhysicsEnabled = true;
		UsePhysicsCollision = true;
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		PhysicsGroup.AddVelocity( StartVelocity * Speed );
		PhysicsBody.GravityEnabled = UseGravity;

		// Delete entity
		if ( RemoveDelay > 0 )
			_ = DeleteAsync( RemoveDelay );
	}

	protected override void OnPhysicsCollision( CollisionEventData eventData )
	{
		base.OnPhysicsCollision( eventData );

		if ( IsSticky && eventData.Entity.IsValid() )
		{
			Velocity = Vector3.Zero;
			Parent = eventData.Entity;
		}
	}

	[Event.Tick.Server]
	public virtual void Tick()
	{
	}
}
