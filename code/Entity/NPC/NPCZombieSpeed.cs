using Sandbox;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

[Spawnable]
[Library("npc_zombie_speed", Title = "Speed Zombie" )]
public partial class NPCZombieSpeed : NPCZombie
{
	public override float Speed => 2000;
	public override float SpawnHealth => 20;
	public override float MeleeStrikeTime => 0.5f;
}
