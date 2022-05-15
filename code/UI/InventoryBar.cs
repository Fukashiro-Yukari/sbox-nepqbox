using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;
using System.Linq;

public partial class InventoryBar : Panel
{
	List<InventoryColumn> columns = new();
	List<Carriable> Weapons = new();
	Carriable SelectedWeapon;

	public InventoryBar()
	{
		for ( int i = 0; i < 9; i++ )
		{
			var icon = new InventoryColumn( i, this );
			columns.Add( icon );
		}
	}

	public override void Tick()
	{
		base.Tick();

		var ply = Local.Pawn as Player;
		if ( ply == null ) return;

		Weapons.Clear();
		Weapons.AddRange( ply.Children.Select( x => x as Carriable ).Where( x => x.IsValid() ) );

		foreach ( var weapon in Weapons )
			columns[weapon.Bucket].UpdateWeapon( weapon );
	}

	TimeSince timeSinceDelay;
	Entity LastActiveChild;

	[Event.BuildInput]
	public void ProcessClientInput( InputBuilder input )
	{
		var ply = Local.Pawn as SandboxPlayer;

		if ( Weapons.Count == 0 || ply == null || ply.LifeState == LifeState.Dead )
		{
			timeSinceDelay = 0f;

			for ( int i = 0; i < 9; i++ )
			{
				columns[i].SetEmpty();
			}

			return;
		}

		var player = Local.Pawn as Player;

		if ( LastActiveChild != player.ActiveChild )
		{
			LastActiveChild = player.ActiveChild;

			if ( player.ActiveChild != SelectedWeapon )
				SelectedWeapon = player.ActiveChild as Weapon;
		}

		if ( SelectedWeapon == null )
			SelectedWeapon = player.ActiveChild as Weapon;

		if ( ply.ActiveChild is PhysGun physgun && physgun.BeamActive ) return;

		var sortedWeapons = Weapons.OrderBy( x => x.Order ).ToList();

		var oldSelectedWeapon = SelectedWeapon;
		int SelectedIndex = sortedWeapons.IndexOf( SelectedWeapon );

		SelectedIndex = SlotPressInput( input, SelectedIndex, sortedWeapons );

		SelectedIndex -= input.MouseWheel;
		SelectedIndex = SelectedIndex.UnsignedMod( Weapons.Count );

		SelectedWeapon = sortedWeapons[SelectedIndex];

		for ( int i = 0; i < 9; i++ )
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

	int SlotPressInput( InputBuilder input, int SelectedIndex, List<Carriable> sortedWeapons )
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

		if ( SelectedWeapon.IsValid() && SelectedWeapon.Bucket == columninput )
		{
			return NextInBucket( sortedWeapons );
		}

		// Are we already selecting a weapon with this column?
		var firstOfColumn = sortedWeapons.Where( x => x.Bucket == columninput ).FirstOrDefault();
		if ( firstOfColumn == null )
		{
			// DOOP sound
			return SelectedIndex;
		}

		return sortedWeapons.IndexOf( firstOfColumn );
	}

	int NextInBucket( List<Carriable> sortedWeapons )
	{
		Assert.NotNull( SelectedWeapon );

		Carriable first = null;
		Carriable prev = null;
		foreach ( var weapon in sortedWeapons.Where( x => x.Bucket == SelectedWeapon.Bucket ) )
		{
			if ( first == null ) first = weapon;
			if ( prev == SelectedWeapon ) return sortedWeapons.IndexOf( weapon );
			prev = weapon;
		}

		return sortedWeapons.IndexOf( first );
	}
}
