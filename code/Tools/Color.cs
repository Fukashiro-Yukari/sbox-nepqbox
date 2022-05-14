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
				var StartPosition = Owner.EyePosition;
				var dir = Owner.EyeRotation.Forward;

				if ( !Input.Pressed( InputButton.Attack1 ) ) return;

				var tr = Trace.Ray( StartPosition, StartPosition + dir * MaxTraceDistance )
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

				new Undo( "Color" ).SetClient( Owner.Client ).Add( new ColorUndo( modelEnt, oldcolor ) ).Finish( $"Color ({modelEnt.RenderColor.ToColor32()})" );

				CreateHitEffects( tr.EndPosition );
			}
		}
	}
}
