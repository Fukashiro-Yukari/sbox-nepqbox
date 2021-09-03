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

	public InventoryColumn(int i, Panel parent)
	{
		Parent = parent;
		Column = i;
		Header = Add.Label($"{i + 1}", "slot-number");
	}

	internal void UpdateWeapon(Entity weapon)
	{
		var icon = ChildrenOfType<InventoryIcon>().FirstOrDefault(x => x.Weapon == weapon);
		if (icon == null)
		{
			icon = new InventoryIcon(weapon);
			icon.Parent = this;
			Icons.Add(icon);
		}
	}

	internal void TickSelection(Entity selectedWeapon, List<Entity> weapons)
	{
		var wep = selectedWeapon as Weapon;
		var car = selectedWeapon as Carriable;
		var somecol = false;

		if (wep != null) somecol = wep?.Bucket == Column;
		else if (car != null) somecol = car?.Bucket == Column;

		SetClass("active", somecol);
		SetClass("empty", weapons.Where(x => 
		{
			Weapon wep = x as Weapon;
			Carriable car = x as Carriable;

			if (wep != null)
				return wep.Bucket == Column;
			else if (car != null)
				return car.Bucket == Column;

			return false;
		}).Count() <= 0);

		for (int i = 0; i < Icons.Count; i++)
		{
			Icons[i].TickSelection(selectedWeapon);
		}
	}

	internal void SetEmpty()
    {
		SetClass("empty", true);
	}
}