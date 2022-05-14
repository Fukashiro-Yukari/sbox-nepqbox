using Sandbox;
using System;

public class ViewModel : BaseViewModel
{
	protected float SwingInfluence => 0.05f;
	protected float ReturnSpeed => 5.0f;
	protected float MaxOffsetLength => 10.0f;
	protected float BobCycleSpeed => 11f;

	private Vector3 swingOffset;
	private float lastPitch;
	private float lastYaw;

	private bool activated = false;
	private float walkBob = 0;

	public bool EnableSwingAndBob = true;

	public float YawInertia { get; private set; }
	public float PitchInertia { get; private set; }

	Angles Bobang;

	public override void PostCameraSetup( ref CameraSetup camSetup )
	{
		base.PostCameraSetup( ref camSetup );

		if ( !Local.Pawn.IsValid() )
			return;

		if ( !activated )
		{
			lastPitch = camSetup.Rotation.Pitch();
			lastYaw = camSetup.Rotation.Yaw();

			YawInertia = 0;
			PitchInertia = 0;

			activated = true;
		}

		Position = camSetup.Position;
		Rotation = camSetup.Rotation;

		var cameraBoneIndex = GetBoneIndex( "camera" );
		if ( cameraBoneIndex != -1 )
		{
			camSetup.Rotation *= (Rotation.Inverse * GetBoneTransform( cameraBoneIndex ).Rotation);
		}

		var pl = Local.Pawn as Player;

		if ( pl.ActiveChild is Weapon wep )
			FieldOfView = wep.FOV;

		camSetup.ViewModel.FieldOfView = FieldOfView;

		var newPitch = Rotation.Pitch();
		var newYaw = Rotation.Yaw();

		PitchInertia = Angles.NormalizeAngle( newPitch - lastPitch );
		YawInertia = Angles.NormalizeAngle( lastYaw - newYaw );

		if ( EnableSwingAndBob )
		{
			var playerVelocity = Local.Pawn.Velocity;

			if ( Local.Pawn is Player player )
			{
				var controller = player.GetActiveController();
				if ( controller != null && controller.HasTag( "noclip" ) )
				{
					playerVelocity = Vector3.Zero;
				}
			}

			var verticalDelta = playerVelocity.z * Time.Delta;
			var viewDown = Rotation.FromPitch( newPitch ).Up * -1.0f;
			verticalDelta *= (1.0f - System.MathF.Abs( viewDown.Cross( Vector3.Down ).y ));
			var pitchDelta = PitchInertia - verticalDelta * 1;
			var yawDelta = YawInertia;

			var offset = CalcSwingOffset( pitchDelta, yawDelta );
			CalcBobbingOffset( ref camSetup );

			Position += Rotation * offset;
		}
		else
		{
			SetAnimParameter( "aim_yaw_inertia", YawInertia );
			SetAnimParameter( "aim_pitch_inertia", PitchInertia );
		}

		lastPitch = newPitch;
		lastYaw = newYaw;
	}

	protected Vector3 CalcSwingOffset( float pitchDelta, float yawDelta )
	{
		Vector3 swingVelocity = new Vector3( 0, yawDelta, pitchDelta );

		swingOffset -= swingOffset * ReturnSpeed * Time.Delta;
		swingOffset += (swingVelocity * SwingInfluence);

		if ( swingOffset.Length > MaxOffsetLength )
		{
			swingOffset = swingOffset.Normal * MaxOffsetLength;
		}

		return swingOffset;
	}

	protected void CalcBobbingOffset( ref CameraSetup camSetup )
	{
		var vel = Owner.Velocity;
		var speed = Math.Clamp( vel.LengthSquared / MathF.Pow( 320, 2 ), 0, 2 );
		var size = speed / 4;
		var dist = Owner.Velocity.Length.LerpInverse( 0, 320 );

		walkBob += Time.Delta * BobCycleSpeed * dist;

		Bobang = Bobang.WithPitch( MathF.Sin( walkBob ) * size );
		Bobang = Bobang.WithYaw( MathF.Sin( walkBob * 2 ) * size );
		Bobang = Bobang.WithRoll( -MathF.Cos( walkBob ) * size );

		Rotation = Rotation.From( Rotation.Angles() + Bobang );

		Position = Position + MathF.Sin( walkBob ) * size * Rotation.Right;
		Position = Position + MathF.Sin( walkBob * 2 ) * size * Rotation.Up;
		Position = Position + -(4 * (size / 2)) * Rotation.Forward;
	}
}
