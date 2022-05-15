using Sandbox;
using System;
using System.Linq;

partial class Inventory : BaseInventory
{
	public Inventory( Player player ) : base( player )
	{
	}

	public override bool CanAdd( Entity entity )
	{
		if ( !entity.IsValid() )
			return false;

		if ( !base.CanAdd( entity ) )
			return false;

		return !IsCarryingType( entity.GetType() );
	}

	public override bool Add( Entity entity, bool makeActive = false )
	{
		if ( !entity.IsValid() )
			return false;

		var player = Owner as SandboxPlayer;
		var weapon = entity as Weapon;
		var notices = !player.SupressPickupNotices;

		if ( IsCarryingType( entity.GetType() ) )
			return false;

		if ( weapon != null && notices && entity.Owner == null )
		{
			Sound.FromWorld( "dm.pickup_weapon", weapon.Position );

			PickupFeed.OnPickup( To.Single( player ), weapon );
		}

		return base.Add( entity, makeActive );
	}

	public bool IsCarryingType( Type t )
	{
		return List.Any( x => x?.GetType() == t );
	}

	public override bool Drop( Entity ent )
	{
		if ( !Host.IsServer )
			return false;

		if ( !Contains( ent ) )
			return false;

		if ( ent is BaseCarriable bc )
		{
			bc.OnCarryDrop( Owner );
		}

		return ent.Parent == null;
	}
}
