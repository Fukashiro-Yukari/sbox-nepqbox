using System;

namespace Sandbox.Tools
{
	[Library( "tool_resizerself", Title = "Resizer Self", Description = "Change the scale of things for yourself", Group = "construction" )]
	public partial class ResizerSelfTool : BaseTool
	{
		public override void Simulate()
		{
			if ( !Host.IsServer )
				return;

			using ( Prediction.Off() )
			{
				int resizeDir = 0;
				var reset = false;

				if ( Input.Down( InputButton.Attack1 ) ) resizeDir = 1;
				else if ( Input.Down( InputButton.Attack2 ) ) resizeDir = -1;
				else if ( Input.Pressed( InputButton.Reload ) ) reset = true;
				else return;

				var scale = reset ? 1.0f : Math.Clamp( Owner.Scale + ((0.5f * Time.Delta) * resizeDir), 0.4f, 4.0f );

				if ( Owner.Scale != scale )
				{
					Owner.Scale = scale;
					Owner.PhysicsGroup?.RebuildMass();
					Owner.PhysicsGroup?.Wake();

					foreach ( var child in Owner.Children )
					{
						if ( !child.IsValid() )
							continue;

						child.PhysicsGroup?.RebuildMass();
						child.PhysicsGroup?.Wake();
					}
				}

				if ( Input.Pressed( InputButton.Attack1 ) || Input.Pressed( InputButton.Attack2 ) || reset )
				{
					Parent.PlaySound( "balloon_pop_cute" );
				}
			}
		}
	}
}
