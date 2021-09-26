using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;

public class InventoryColumn : Panel
{
	public int Column;
	public Label Header;

	internal List<InventoryIcon> Icons = new();

	public InventoryColumn( int i, Panel parent )
	{
		Parent = parent;
		Column = i;
		Header = Add.Label( $"{i + 1}", "slot-number" );
	}

	internal void UpdateWeapon( Carriable weapon )
	{
		var icon = ChildrenOfType<InventoryIcon>().FirstOrDefault( x => x.Weapon == weapon );
		if ( icon == null )
		{
			icon = new InventoryIcon( weapon );
			icon.Parent = this;
			Icons.Add( icon );
		}
	}

	internal void TickSelection( List<Carriable> weapons )
	{
		var wep = Local.Pawn.ActiveChild as Carriable;

		SetClass( "active", wep?.Bucket == Column );
		SetClass( "empty", weapons.Where( x => x.Bucket == Column ).Count() <= 0 );

		for ( int i = 0; i < Icons.Count; i++ )
		{
			Icons[i].TickSelection();
		}
	}

	internal void SetEmpty()
    {
		SetClass("empty", true);
	}
}
