namespace Sandbox.Tools
{
	[Library( "tool_spectate_player", Title = "Spectate Player", Description = "View other players", Group = "fun" )]
	public partial class SpectatePlayerTool : BaseTool
	{
		private SandboxPlayer SPlayer;
		private BaseViewModel ViewModelEntity;
		private PawnController LastController;

		public override void Simulate()
		{
			if ( SPlayer != null )
			{
				if ( SPlayer.LifeState == LifeState.Dying )
					Reset();

				if ( !Owner.IsLocalPawn ) return;
				if ( SPlayer.ActiveChild == null )
				{
					ViewModelEntity?.Delete();
					ViewModelEntity = null;

					return;
				}

				var splywep = SPlayer.ActiveChild as BaseCarriable;

				if ( splywep == null ) return;
				if ( ViewModelEntity != null )
					CloneViewModel( splywep.ViewModelPath );
				else
					CreateViewModel( splywep.ViewModelPath );

				return;
			}

			if ( !Input.Pressed( InputButton.Attack1 ) ) return;

			var startPos = Owner.EyePos;
			var dir = Owner.EyeRot.Forward;

			var tr = Trace.Ray( startPos, startPos + dir * MaxTraceDistance )
				.Ignore( Owner )
				.HitLayer( CollisionLayer.Debris )
				.Run();

			if ( !tr.Hit || !tr.Entity.IsValid() ) return;
			if ( !(tr.Entity is SandboxPlayer) ) return;

			CreateHitEffects( tr.EndPos );

			LastController = Owner.Controller;
			Owner.Controller = null;

			if ( Owner.IsLocalPawn )
				Parent.ViewModelEntity.EnableDrawing = false;

			SPlayer = tr.Entity as SandboxPlayer;
			var SOwner = Owner as SandboxPlayer;

			if ( SOwner == null ) return;

			SOwner.LastCamera = SOwner.MainCamera;
			SOwner.MainCamera = new SpectateFirstPersonCamera( SPlayer );
			SOwner.Camera = SOwner.MainCamera;

			if ( SPlayer.ActiveChild == null ) return;

			var splywep2 = SPlayer.ActiveChild as BaseCarriable;

			if ( splywep2 == null ) return;
			if ( Owner.IsLocalPawn )
				CreateViewModel( splywep2.ViewModelPath );
		}

		private void CreateViewModel( string CloneViewModelPath )
		{
			Host.AssertClient();

			ViewModelEntity = new ViewModel
			{
				Position = Parent.Position,
				Owner = Owner,
				EnableViewmodelRendering = true
			};

			ViewModelEntity.SetModel( CloneViewModelPath );
		}

		private void CloneViewModel( string CloneViewModelPath )
		{
			Host.AssertClient();

			ViewModelEntity.SetModel( CloneViewModelPath );
		}

		private void Reset()
		{
			SPlayer = null;
			var SOwner = Owner as SandboxPlayer;

			if ( SOwner == null ) return;
			if ( SOwner.LifeState == LifeState.Dying || SOwner.LifeState == LifeState.Dead )
			{
				SOwner.LastCamera = new FirstPersonCamera();
			}
			else
			{
				SOwner.LastCamera = new FirstPersonCamera();
				SOwner.MainCamera = SOwner.LastCamera;
				SOwner.Camera = SOwner.MainCamera;
			}

			ViewModelEntity?.Delete();
			ViewModelEntity = null;

			if ( LastController != null )
				Owner.Controller = LastController;
		}

		public override void Deactivate()
		{
			base.Deactivate();

			Reset();
		}
	}
}
