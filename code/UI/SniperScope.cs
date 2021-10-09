
using Sandbox;
using Sandbox.UI;

public class SniperScope : Panel
{
    Image Lens;
    Image Scope;

    Panel LeftBar;
    Panel RightBar;
    Panel TopBar;
    Panel BottomBar;

	public SniperScope( string lensTexture, string scopeTexture )
	{
		StyleSheet.Load( "/UI/SniperScope.scss" );

		LeftBar = Add.Panel( "leftBar" );
		RightBar = Add.Panel( "rightBar" );
		TopBar = Add.Panel( "topBar" );
		BottomBar = Add.Panel( "bottomBar" );

		AddChild( out Lens, "lens" );
		Lens.SetTexture( lensTexture );

		AddChild( out Scope, "scope" );
		Scope.SetTexture( scopeTexture );
	}

	public override void Tick()
	{
		base.Tick();

		var player = Local.Pawn;
		if ( player == null ) return;

		var weapon = player.ActiveChild as WeaponSniper;

		// Show when zooming
		Style.Opacity = (weapon == null || weapon.ZoomLevel <= -1) ? 0 : 1;
		Style.Dirty();

		Lens.PositionAtCrosshair();
		Scope.PositionAtCrosshair();
		LeftBar.PositionAtCrosshair();
		RightBar.PositionAtCrosshair();
		TopBar.PositionAtCrosshair();
		BottomBar.PositionAtCrosshair();

		// Scope
		var scopeSize = Screen.Height * ScaleFromScreen * 0.9f;
		Lens.Style.Width = scopeSize;
		Lens.Style.Height = scopeSize;
		Lens.Style.Dirty();
		Scope.Style.Width = scopeSize;
		Scope.Style.Height = scopeSize;
		Scope.Style.Dirty();
	}
}
