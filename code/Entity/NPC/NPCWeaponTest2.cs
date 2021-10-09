using Sandbox;
using System.Linq;

[Library( "npc_weapon_test2", Title = "Weapon Test 2", Spawnable = true )]
public partial class NPCWeaponTest2 : NPC
{
	public override bool UseWeapon => true;

	public override void Spawn()
	{
		base.Spawn();

		Inventory.Add( new AK47(), true );
	}

	public override void OnTick()
	{
		if ( ActiveChild is Weapon wep && wep.CanNPCPrimaryAttack() )
		{
			wep.NPCAttackPrimary();
		}
	}
}
