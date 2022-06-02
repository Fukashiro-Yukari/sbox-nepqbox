namespace Sandbox.Tools
{
	[Library( "tool_zombie_spawner", Title = "Zombie Spawner", Description = "Just spawn the zombie", Group = "fun" )]
	public partial class ZombieSpawner : BaseTool
	{
		PreviewEntity previewModel;

		protected override bool IsPreviewTraceValid( TraceResult tr )
		{
			if ( !base.IsPreviewTraceValid( tr ) )
				return false;

			if ( tr.Entity is Zombie )
				return false;

			return true;
		}

		public override void CreatePreviews()
		{
			if ( TryCreatePreview( ref previewModel, "models/citizen/citizen.vmdl" ) )
			{
				previewModel.RelativeToNormal = false;
				previewModel.RenderColor = new Color32( (byte)(105 + Rand.Int( 20 )), (byte)(174 + Rand.Int( 20 )), (byte)(59 + Rand.Int( 20 )), 255 ).ToColor();
			}
		}

		public override void Simulate()
		{
			if ( !Host.IsServer )
				return;

			using ( Prediction.Off() )
			{
				bool isNormalZombie = Input.Pressed( InputButton.PrimaryAttack );
				bool isSpeedZombie = Input.Pressed( InputButton.SecondaryAttack );

				if ( isNormalZombie || isSpeedZombie )
				{
					var StartPosition = Owner.EyePosition;
					var dir = Owner.EyeRotation.Forward;

					var tr = Trace.Ray( StartPosition, StartPosition + dir * MaxTraceDistance )
						.Ignore( Owner )
						.Run();

					if ( !tr.Hit || !tr.Entity.IsValid() )
						return;

					CreateHitEffects( tr.EndPosition );

					var zombie = isSpeedZombie ? new ZombieSpeed() : new Zombie();

					zombie.Position = tr.EndPosition;

					new Undo( "Zombie" ).SetClient( Owner.Client ).AddEntity( zombie ).Finish( isSpeedZombie ? "Speed Zombie" : "Zombie" );
				}
			}
		}
	}
}
