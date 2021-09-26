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

			activated = true;
		}

		Position = camSetup.Position;
		Rotation = camSetup.Rotation;

		if ( Local.Pawn.ActiveChild is Weapon wep )
		{
			FieldOfView = wep.FOV;
		}

		camSetup.ViewModel.FieldOfView = FieldOfView;

		var playerVelocity = Local.Pawn.Velocity;

		if ( Local.Pawn is Player player )
		{
			var controller = player.GetActiveController();
			if ( controller != null && controller.HasTag( "noclip" ) )
			{
				playerVelocity = Vector3.Zero;
			}
		}

		var newPitch = Rotation.Pitch();
		var newYaw = Rotation.Yaw();

		var pitchDelta = Angles.NormalizeAngle( newPitch - lastPitch );
		var yawDelta = Angles.NormalizeAngle( lastYaw - newYaw );

		var verticalDelta = playerVelocity.z * Time.Delta;
		var viewDown = Rotation.FromPitch( newPitch ).Up * -1.0f;
		verticalDelta *= (1.0f - System.MathF.Abs( viewDown.Cross( Vector3.Down ).y ));
		pitchDelta -= verticalDelta * 1;

		var offset = CalcSwingOffset( pitchDelta, yawDelta );

		Position += Rotation * offset;

		lastPitch = newPitch;
		lastYaw = newYaw;

		CalcBobbingOffset( ref camSetup );
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
