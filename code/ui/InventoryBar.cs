using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The main inventory panel, top left of the screen.
/// </summary>
public class InventoryBar : Panel
{
	List<InventoryColumn> columns = new();
	List<Entity> Weapons = new();

	public bool IsOpen;
	Entity SelectedWeapon;

	[ConVar.ClientData( "cl_fast_switch" )]
	public static bool FastSwitch { get; set; } = true;

	//TimeSince CloseMenuTime;

	private float CloseMenuTime = -1;

	public InventoryBar()
	{
		for ( int i=0; i<9; i++ )
		{
			var icon = new InventoryColumn( i, this );
			columns.Add( icon );
		}

		if ( CloseMenuTime < 0 )
		{
			CloseMenuTime = Time.Tick + 100;
		}
	}

	public override void Tick()
	{
		base.Tick();

		SetClass( "active", IsOpen );

		var player = Local.Pawn as Player;
		if ( player == null ) return;

		Weapons.Clear();
		Weapons.AddRange( player.Children.Select( x => x as Weapon ).Where( x => x.IsValid() ) );
		Weapons.AddRange( player.Children.Select( x => x as Carriable ).Where( x => x.IsValid() ) );

		foreach ( var weapon in Weapons )
		{
			Weapon wep = weapon as Weapon;
			Carriable car = weapon as Carriable;

			if (wep != null)
				columns[wep.Bucket].UpdateWeapon( wep );
			else if ( car != null)
				columns[car.Bucket].UpdateWeapon( car );
		}
	}

	/// <summary>
	/// IClientInput implementation, calls during the client input build.
	/// You can both read and write to input, to affect what happens down the line.
	/// </summary>
	[Event.BuildInput]
	public void ProcessClientInput( InputBuilder input )
	{
		bool wantOpen = IsOpen;

		// If we're not open, maybe this input has something that will 
		// make us want to start being open?
		wantOpen = wantOpen || input.MouseWheel != 0;
		wantOpen = wantOpen || input.Pressed( InputButton.Slot1 );
		wantOpen = wantOpen || input.Pressed( InputButton.Slot2 );
		wantOpen = wantOpen || input.Pressed( InputButton.Slot3 );
		wantOpen = wantOpen || input.Pressed( InputButton.Slot4 );
		wantOpen = wantOpen || input.Pressed( InputButton.Slot5 );
		wantOpen = wantOpen || input.Pressed( InputButton.Slot6 );
		wantOpen = wantOpen || input.Pressed( InputButton.Slot7 );
		wantOpen = wantOpen || input.Pressed( InputButton.Slot8 );
		wantOpen = wantOpen || input.Pressed( InputButton.Slot9 );

		if ( Weapons.Count == 0)
		{
			IsOpen = false;
			return;
		}

		if ( Local.Pawn.ActiveChild is PhysGun && input.Down( InputButton.Attack1 ) && !input.Pressed( InputButton.Attack1 ) )
		{
			IsOpen = false;
			return;
		}

		// We're not open, but we want to be
		if ( IsOpen != wantOpen )
		{
			SelectedWeapon = Local.Pawn.ActiveChild;
			IsOpen = true;
		}

		// Not open fuck it off
		if ( !IsOpen ) return;

		if ( FastSwitch )
		{
			input.ActiveChild = SelectedWeapon;
		}

		//
		// Fire pressed when we're open - select the weapon and close.
		//
		if ( input.Down( InputButton.Attack1 ) )
		{
			input.SuppressButton( InputButton.Attack1 );

			if ( !FastSwitch )
				input.ActiveChild = SelectedWeapon;

			IsOpen = false;
			Sound.FromScreen( "dm.ui_select" );

			return;
		}

		// get our current index
		var oldSelected = SelectedWeapon;
		int SelectedIndex = Weapons.IndexOf( SelectedWeapon );

		SelectedIndex = SlotPressInput( input, SelectedIndex );

		// forward if mouse wheel was pressed
		SelectedIndex += input.MouseWheel;
		SelectedIndex = SelectedIndex.UnsignedMod( Weapons.Count );

		SelectedWeapon = Weapons[SelectedIndex];

		for ( int i = 0; i < 9; i++ )
		{
			columns[i].TickSelection( SelectedWeapon );
		}

		input.MouseWheel = 0;

		if ( oldSelected  != SelectedWeapon )
		{
			Sound.FromScreen( "dm.ui_tap" );
		}
	}

	int SlotPressInput( InputBuilder input, int SelectedIndex )
	{
		var columninput = -1;

		if ( input.Pressed( InputButton.Slot1 ) ) columninput = 0;
		if ( input.Pressed( InputButton.Slot2 ) ) columninput = 1;
		if ( input.Pressed( InputButton.Slot3 ) ) columninput = 2;
		if ( input.Pressed( InputButton.Slot4 ) ) columninput = 3;
		if ( input.Pressed( InputButton.Slot5 ) ) columninput = 4;
		if ( input.Pressed( InputButton.Slot6 ) ) columninput = 5;
		if ( input.Pressed( InputButton.Slot7 ) ) columninput = 6;
		if ( input.Pressed( InputButton.Slot8 ) ) columninput = 7;
		if ( input.Pressed( InputButton.Slot9 ) ) columninput = 8;

		if ( columninput == -1 ) return SelectedIndex;

		Weapon wep = SelectedWeapon as Weapon;
		Carriable car = SelectedWeapon as Carriable;

		if (wep != null )
		{
			if ( wep.IsValid() && wep.Bucket == columninput )
			{
				return NextInBucket();
			}
		}
		else if (car != null )
		{
			if ( car.IsValid() && car.Bucket == columninput )
			{
				return NextInBucket();
			}
		}

		// Are we already selecting a weapon with this column?
		var firstOfColumn = Weapons.Where( x => 
		{
			Weapon wep = x as Weapon;
			Carriable car = x as Carriable;

			if (wep != null) return wep.Bucket == columninput;
			else if (car != null) return car.Bucket == columninput;

			return false;
		} ).OrderBy( x => 
		{
			Weapon wep = x as Weapon;
			Carriable car = x as Carriable;

			if (wep != null) return wep.BucketWeight;
			else if ( car != null ) return car.BucketWeight;

			return -1;
		} ).FirstOrDefault();
		if ( firstOfColumn == null )
		{
			// DOOP sound
			return SelectedIndex;
		}

		return Weapons.IndexOf( firstOfColumn );
	}

	int NextInBucket()
	{
		Assert.NotNull( SelectedWeapon );

		Entity first = null;
		Entity prev = null;

		foreach ( var weapon in Weapons.Where( x => 
		{
			Weapon swep = SelectedWeapon as Weapon;
			Carriable scar = SelectedWeapon as Carriable;
			Weapon we = x as Weapon;
			Carriable ca = x as Carriable;

			if ( swep != null && we != null ) return we.Bucket == swep.Bucket;
			else if ( swep != null && ca != null ) return ca.Bucket == swep.Bucket;
			else if ( scar != null && we != null ) return we.Bucket == scar.Bucket;
			else if ( scar != null && ca != null ) return ca.Bucket == scar.Bucket;

			return false;
		} ).OrderBy( x => 
		{
			Weapon we = x as Weapon;
			Carriable ca = x as Carriable;

			if ( we != null ) return we.BucketWeight;
			else if (ca != null) return ca.BucketWeight;

			return -1;
		} ))
		{
			if ( first == null ) first = weapon;
			if ( prev == SelectedWeapon ) return Weapons.IndexOf( weapon );
			prev = weapon;
		}

		return Weapons.IndexOf( first );
	}
}
