using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;
using System;
using System.Linq;

public partial class InventoryBar : Panel
{
	public static InventoryBar Current;

	List<InventoryColumn> columns = new();
    List<Entity> Weapons = new();
    Entity SelectedWeapon;

	public InventoryBar()
    {
		Current = this;

		for (int i = 0; i < 9; i++)
        {
            var icon = new InventoryColumn(i, this);
            columns.Add(icon);
        }
    }

	public override void Tick()
    {
        base.Tick();

        var ply = Local.Pawn as Player;
        if (ply == null) return;

        Weapons.Clear();
        Weapons.AddRange(ply.Children.Select(x => x as Weapon).Where(x => x.IsValid()));
        Weapons.AddRange(ply.Children.Select(x => x as Carriable).Where(x => x.IsValid()));

        foreach (var weapon in Weapons)
        {
            Weapon wep = weapon as Weapon;
            Carriable car = weapon as Carriable;

			if ( wep != null )
				columns[wep.Bucket].UpdateWeapon( wep );
			else if ( car != null )
				columns[car.Bucket].UpdateWeapon( car );
        }
    }

	TimeSince timeSinceDelay;
	Entity LastActiveChild;

	[Event.BuildInput]
    public void ProcessClientInput(InputBuilder input)
    {
		var ply = Local.Pawn as SandboxPlayer;

		if ( Weapons.Count == 0 || ply == null || ply.LifeState == LifeState.Dead )
        {
            timeSinceDelay = 0f;

            for (int i = 0; i < 9; i++)
            {
                columns[i].SetEmpty();
            }

            return;
        }

		if ( LastActiveChild != Local.Pawn.ActiveChild )
		{
			LastActiveChild = Local.Pawn.ActiveChild;

			if ( Local.Pawn.ActiveChild != SelectedWeapon )
				SelectedWeapon = Local.Pawn.ActiveChild;
		}

		if ( ply.ActiveChild is PhysGun physgun && physgun.BeamActive ) return;
		if ( SelectedWeapon == null )
            SelectedWeapon = Local.Pawn.ActiveChild;

		var oldSelectedWeapon = SelectedWeapon;
		int SelectedIndex = Weapons.IndexOf(SelectedWeapon);

        SelectedIndex = SlotPressInput(input, SelectedIndex);

        SelectedIndex -= input.MouseWheel;
        SelectedIndex = SelectedIndex.UnsignedMod(Weapons.Count);

        SelectedWeapon = Weapons[SelectedIndex];

        for (int i = 0; i < 9; i++)
        {
			columns[i].TickSelection( Weapons );
        }

        input.MouseWheel = 0;

		if ( oldSelectedWeapon != SelectedWeapon )
		{
			if ( timeSinceDelay > 0.01 )
				input.ActiveChild = SelectedWeapon;
		}
    }

    int SlotPressInput(InputBuilder input, int SelectedIndex)
    {
        var columninput = -1;

        if (input.Pressed(InputButton.Slot1)) columninput = 0;
        if (input.Pressed(InputButton.Slot2)) columninput = 1;
        if (input.Pressed(InputButton.Slot3)) columninput = 2;
        if (input.Pressed(InputButton.Slot4)) columninput = 3;
        if (input.Pressed(InputButton.Slot5)) columninput = 4;
        if (input.Pressed(InputButton.Slot6)) columninput = 5;
        if (input.Pressed(InputButton.Slot7)) columninput = 6;
        if (input.Pressed(InputButton.Slot8)) columninput = 7;
        if (input.Pressed(InputButton.Slot9)) columninput = 8;

        if (columninput == -1) return SelectedIndex;

        Weapon wep = Local.Pawn.ActiveChild as Weapon;
        Carriable car = Local.Pawn.ActiveChild as Carriable;

        if (wep != null)
        {
            if (wep.IsValid() && wep.Bucket == columninput)
            {
                return NextInBucket();
            }
        }
        else if (car != null)
        {
            if (car.IsValid() && car.Bucket == columninput)
            {
                return NextInBucket();
            }
        }

        // Are we already selecting a weapon with this column?
        var firstOfColumn = Weapons.Where(x =>
        {
            Weapon wep = x as Weapon;
            Carriable car = x as Carriable;

            if (wep != null) return wep.Bucket == columninput;
            else if (car != null) return car.Bucket == columninput;

            return false;
        }).OrderBy(x =>
        {
            Weapon wep = x as Weapon;
            Carriable car = x as Carriable;

            if (wep != null) return wep.BucketWeight;
            else if (car != null) return car.BucketWeight;

            return -1;
        }).FirstOrDefault();
        if (firstOfColumn == null)
        {
            // DOOP sound
            return SelectedIndex;
        }

        return Weapons.IndexOf(firstOfColumn);
    }

    int NextInBucket()
	{
        Entity first = null;
        Entity prev = null;

        foreach (var weapon in Weapons.Where(x =>
        {
            Weapon swep = Local.Pawn.ActiveChild as Weapon;
            Carriable scar = Local.Pawn.ActiveChild as Carriable;
            Weapon we = x as Weapon;
            Carriable ca = x as Carriable;

            if (swep != null && we != null) return we.Bucket == swep.Bucket;
            else if (swep != null && ca != null) return ca.Bucket == swep.Bucket;
            else if (scar != null && we != null) return we.Bucket == scar.Bucket;
            else if (scar != null && ca != null) return ca.Bucket == scar.Bucket;

            return false;
        }).OrderBy(x =>
        {
            Weapon we = x as Weapon;
            Carriable ca = x as Carriable;

            if (we != null) return we.BucketWeight;
            else if (ca != null) return ca.BucketWeight;

            return -1;
        }))
        {
            if (first == null) first = weapon;
            if (prev == Local.Pawn.ActiveChild ) return Weapons.IndexOf(weapon);
            prev = weapon;
        }

        return Weapons.IndexOf(first);
    }
}
