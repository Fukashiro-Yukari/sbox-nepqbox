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

			if ( tr.Entity is NPCZombie )
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
				bool isNormalZombie = Input.Pressed( InputButton.Attack1 );
				bool isSpeedZombie = Input.Pressed( InputButton.Attack2 );

				if ( isNormalZombie || isSpeedZombie )
				{
					var startPos = Owner.EyePos;
					var dir = Owner.EyeRot.Forward;

					var tr = Trace.Ray( startPos, startPos + dir * MaxTraceDistance )
						.Ignore( Owner )
						.Run();

					if ( !tr.Hit || !tr.Entity.IsValid() )
						return;

					CreateHitEffects( tr.EndPos );

					var zombie = isSpeedZombie ? new NPCZombieSpeed() : new NPCZombie();

					zombie.Position = tr.EndPos;

					new Undo( "Zombie" ).SetClient( Owner.GetClientOwner() ).AddEntity( zombie ).Finish( isSpeedZombie ? "Speed Zombie" : "Zombie" );
				}
			}
		}
	}
}
