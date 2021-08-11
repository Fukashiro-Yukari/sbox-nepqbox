
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

class InventoryIcon : Panel
{
	public Entity Weapon;
	public Panel Icon;
	public Label Label;

	public InventoryIcon( Entity weapon )
	{
		Weapon = weapon;
		Icon = Add.Panel( "icon" );
		Label = Add.Label( "", "item-name" );
		AddClass( weapon.ClassInfo.Name );
	}

	internal void TickSelection( Entity selectedWeapon, bool SomeColumn )
	{
		Label.SetText( SomeColumn ? Weapon.ClassInfo.Title : "" );

		SetClass( "active", selectedWeapon == Weapon );
		SetClass( "empty", false );
	}

	public override void Tick()
	{
		base.Tick();

		if ( !Weapon.IsValid() || Weapon.Owner != Local.Pawn )
			Delete();
	}
}
