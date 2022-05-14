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
		var tool = GetCurrentFireMode();
		SetClass( "active", tool != null );

		if ( tool != null )
		{
			Title.SetText( tool.ClassInfo.Title );
			Description.SetText( tool.ClassInfo.Description );
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
