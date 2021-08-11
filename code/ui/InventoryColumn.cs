
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;

public class InventoryColumn : Panel
{
	public int Column;
	public bool IsSelected;
	public Label Header;
	public int SelectedIndex;

	internal List<InventoryIcon> Icons = new();

	public InventoryColumn( int i, Panel parent )
	{
		Parent = parent;
		Column = i;
		Header = Add.Label( $"{i+1}", "slot-number" );
	}

	internal void UpdateWeapon( Entity weapon )
	{
		var icon = ChildrenOfType<InventoryIcon>().FirstOrDefault( x => x.Weapon == weapon );
		if ( icon == null )
		{
			icon = new InventoryIcon( weapon );
			icon.Parent = this;
			Icons.Add( icon );
		}
	}

	internal void TickSelection( Entity selectedWeapon )
	{
		Weapon wep = selectedWeapon as Weapon;
		Carriable car = selectedWeapon as Carriable;
		var somecol = false;

		if (wep != null) somecol = wep?.Bucket == Column;
		else if (car != null) somecol =  car?.Bucket == Column;

		SetClass( "active", somecol );

		for ( int i=0; i< Icons.Count; i++ )
		{
			Icons[i].TickSelection( selectedWeapon, somecol );
		}
	}
}
