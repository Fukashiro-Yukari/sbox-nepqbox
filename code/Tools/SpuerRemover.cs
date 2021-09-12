namespace Sandbox.Tools
{
	[Library( "tool_spuerremover", Title = "Spuer Remover", Description = "Remove entities, more entities can be remove", Group = "construction" )]
	public partial class SpuerRemoverTool : BaseTool
	{
		public override void Simulate()
		{
			if ( !Host.IsServer )
				return;

			using ( Prediction.Off() )
			{
				if ( !Input.Pressed( InputButton.Attack1 ) )
					return;

				var startPos = Owner.EyePos;
				var dir = Owner.EyeRot.Forward;

				var tr = Trace.Ray( startPos, startPos + dir * MaxTraceDistance )
					.Ignore( Owner )
					.HitLayer( CollisionLayer.Debris )
					.Run();

				if ( !tr.Hit || !tr.Entity.IsValid() )
					return;

				if ( tr.Entity is Player )
					return;

				CreateHitEffects( tr.EndPos );

				if ( tr.Entity is WorldEntity )
					return;

				tr.Entity.Delete();

				var particle = Particles.Create( "particles/physgun_freeze.vpcf" );
				particle.SetPosition( 0, tr.Entity.Position );
			}
		}
	}
}
