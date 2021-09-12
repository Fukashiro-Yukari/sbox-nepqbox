using System;

namespace Sandbox.Tools
{
	[Library( "tool_color", Title = "Color", Description = "Change render color and alpha of entities", Group = "construction" )]
	public partial class ColorTool : BaseTool
	{
		public override void Simulate()
		{
			if ( !Host.IsServer )
				return;

			using ( Prediction.Off() )
			{
				var startPos = Owner.EyePos;
				var dir = Owner.EyeRot.Forward;

				if ( !Input.Pressed( InputButton.Attack1 ) ) return;

				var tr = Trace.Ray( startPos, startPos + dir * MaxTraceDistance )
				   .Ignore( Owner )
				   .UseHitboxes()
				   .HitLayer( CollisionLayer.Debris )
				   .Run();

				if ( !tr.Hit || !tr.Entity.IsValid() )
					return;

				if ( tr.Entity is not ModelEntity modelEnt )
					return;

				var oldcolor = modelEnt.RenderColor;

				modelEnt.RenderColor = Color.Random;

				new Undo( "Color" ).SetClient( Owner.GetClientOwner() ).Add( new ColorUndo( modelEnt, oldcolor ) ).Finish( $"Color ({modelEnt.RenderColor.ToColor32()})" );

				CreateHitEffects( tr.EndPos );
			}
		}
	}
}
