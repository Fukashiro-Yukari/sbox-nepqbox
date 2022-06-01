using Sandbox;
using Sandbox.CWEP;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class CurrentFireMode : Panel
{
	public Label Title;
	public Label Description;

	public CurrentFireMode()
	{
		Title = Add.Label( "Tool", "title" );
		Description = Add.Label( "This is a tool", "description" );
	}

	public override void Tick()
	{
		var mode = GetCurrentFireMode();
		SetClass( "active", mode != null );

		if ( mode != null )
		{
			var display = DisplayInfo.For( mode );

			Title.SetText( display.Name );
			Description.SetText( display.Description );
		}
	}

	CWEPFireMode GetCurrentFireMode()
	{
		var player = Local.Pawn as Player;
		if ( player == null ) return null;

		var inventory = player.Inventory;
		if ( inventory == null ) return null;

		if ( inventory.Active is not CWEPW cwep ) return null;

		return cwep?.CurrentFireMode;
	}
}
