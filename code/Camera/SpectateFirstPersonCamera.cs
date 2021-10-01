namespace Sandbox
{
	public partial class SpectateFirstPersonCamera : Camera
	{
		Vector3 lastPos;
		Player viewPlayer;

		public SpectateFirstPersonCamera( Player viewply )
		{
			if ( !(viewply is Player) ) return;

			viewPlayer = viewply;
		}

		public override void Activated()
		{
			var pawn = viewPlayer;
			if ( pawn == null ) return;

			Pos = pawn.EyePos;
			Rot = pawn.EyeRot;

			lastPos = Pos;
		}

		public override void Update()
		{
			var pawn = viewPlayer;
			if ( pawn == null ) return;

			var eyePos = pawn.EyePos;
			if ( eyePos.Distance( lastPos ) < 300 ) // TODO: Tweak this, or add a way to invalidate lastpos when teleporting
			{
				Pos = Vector3.Lerp( eyePos.WithZ( lastPos.z ), eyePos, 20.0f * Time.Delta );
			}
			else
			{
				Pos = eyePos;
			}

			Rot = pawn.EyeRot;

			FieldOfView = 80;

			Viewer = pawn;
			lastPos = Pos;
		}
	}
}
