namespace Sandbox
{
	[Library]
	public class SandboxFlyingController : BasePlayerController
	{
		public float Bounce { get; set; } = 0.25f;
		public float Size { get; set; } = 20.0f;

		public override void Simulate()
		{
			Rotation = Input.Rotation;

			var vel = (Input.Rotation.Forward * Input.Forward) + (Input.Rotation.Left * Input.Left);

			if ( Input.Down( InputButton.Jump ) )
			{
				vel += Vector3.Up * 1;
			}

			vel = vel.Normal * 2000;

			if ( Input.Down( InputButton.Run ) )
				vel *= 5.0f;

			if ( Input.Down( InputButton.Duck ) )
				vel *= 0.2f;

			Velocity += vel * Time.Delta;

			if ( Velocity.LengthSquared > 0.01f )
			{
				Move( Velocity * Time.Delta );
			}

			Velocity = Velocity.Approach( 0, Velocity.Length * Time.Delta * 5.0f );

			EyeRot = Input.Rotation;
			WishVelocity = Velocity;
			GroundEntity = null;
			BaseVelocity = Vector3.Zero;
		}

		public void Move( Vector3 delta, int a = 0 )
		{
			if ( a > 1 )
				return;

			var len = delta.Length;
			var targetPos = Position + delta;
			var tr = Trace.Ray( Position, targetPos ).WorldOnly().Size( Size ).Run();

			if ( tr.StartedSolid )
			{
				Position = targetPos;
			}
			else if ( tr.Hit )
			{
				Position = tr.EndPos + tr.Normal;

				// subtract the normal from our velocity
				Velocity = Velocity.SubtractDirection( tr.Normal * (1.0f + Bounce) );

				delta = Velocity.Normal * delta.Length * (1.0f - tr.Fraction);

				Move( delta, ++a );
			}
			else
			{
				Position = tr.EndPos;
			}
		}
	}
}
