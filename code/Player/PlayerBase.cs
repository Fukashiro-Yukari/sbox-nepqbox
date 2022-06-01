using System;
using Sandbox;

partial class PlayerBase : Player
{
	private TimeSince timeSinceShake;
	private ScreenShakeStruct lastScreenShake;
	private float nextShake;

	public override void PostCameraSetup( ref CameraSetup camSetup )
	{
		base.PostCameraSetup( ref camSetup );

		if ( timeSinceShake < lastScreenShake.Length && timeSinceShake > nextShake )
		{
			var random = new Random();
			camSetup.Position += new Vector3( random.Float( 0, lastScreenShake.Size ), random.Float( 0, lastScreenShake.Size ), random.Float( 0, lastScreenShake.Size ) );
			camSetup.Rotation *= Rotation.From( new Angles( random.Float( 0, lastScreenShake.Rotation ), random.Float( 0, lastScreenShake.Rotation ), 0 ) );

			nextShake = timeSinceShake + lastScreenShake.Delay;
		}
	}

	public virtual void ScreenShake( ScreenShakeStruct screenShake )
	{
		lastScreenShake = screenShake;
		timeSinceShake = 0;
		nextShake = 0;
	}

	[ConCmd.Server( "inventory_current" )]
	public static void SetInventoryCurrent( string entName )
	{
		var target = ConsoleSystem.Caller.Pawn as Player;
		if ( target == null ) return;

		var inventory = target.Inventory;
		if ( inventory == null )
			return;

		for ( int i = 0; i < inventory.Count(); ++i )
		{
			var slot = inventory.GetSlot( i );
			if ( !slot.IsValid() )
				continue;

			if ( slot.ClassName != entName )
				continue;

			inventory.SetActiveSlot( i, false );

			break;
		}
	}
}
