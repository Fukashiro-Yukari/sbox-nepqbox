
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

class InventoryIcon : Panel
{
	public Entity Weapon;
	public Image Icon;
	public Label Label;

	public InventoryIcon( Entity weapon )
	{
		Weapon = weapon;
		Label = Add.Label( "", "item-name" );

		AddChild(out Icon, "icon");

		Weapon wep = weapon as Weapon;
		Carriable car = weapon as Carriable;

		if (wep != null)
			Icon.SetTexture(wep.Icon);
		else if (car != null)
			Icon.SetTexture(car.Icon);
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
