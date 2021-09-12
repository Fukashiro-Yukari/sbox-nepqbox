using Sandbox;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

[Library("npc_zombie", Title = "Zombie", Spawnable = true )]
public partial class NpcZombie : Npc
{
	public override float Speed => 500;
	public override float SpawnHealth => 50;
    public override bool HaveDress => false;

	SandboxPlayer Target;

	public override void Spawn()
	{
		base.Spawn();

		SetMaterialGroup(3);

		RenderColor = new Color32((byte)(105 + Rand.Int(20)), (byte)(174 + Rand.Int(20)), (byte)(59 + Rand.Int(20)), 255).ToColor();

		ZombieClothes();
	}

	public virtual void ZombieClothes()
    {
		if (true)
		{
			var model = Rand.FromArray(new[]
			{
				"models/citizen_clothes/trousers/trousers.jeans.vmdl",
				"models/citizen_clothes/trousers/trousers.lab.vmdl"
			});

			AddClothing( model );
		}

		if (true)
		{
			var model = Rand.FromArray(new[]
			{
				"models/citizen_clothes/shirt/shirt_longsleeve.plain.vmdl",
				"models/citizen_clothes/shirt/shirt_longsleeve.police.vmdl",
				"models/citizen_clothes/shirt/shirt_longsleeve.scientist.vmdl",
			});

			if (Rand.Int(3) == 1)
				AddClothing( model );
		}

		if (Rand.Int(3) == 1)
			AddClothing( "models/citizen_clothes/hair/hair_femalebun.black.vmdl" );
		else if (Rand.Int(10) == 1)
			AddClothing( "models/citizen_clothes/hat/hat_hardhat.vmdl" );

		SetBodyGroup(1, 0);
	}

	private void FindTarget()
    {
		var rply = All.OfType<SandboxPlayer>().ToArray();

		Target = rply[Rand.Int( 0, rply.Count() - 1 )];
    }

	public override void OnTick()
	{
		if (Target == null || Target.LifeState == LifeState.Dead)
			FindTarget();
		else
        {
			Steer = new NavSteer();
			Steer.Target = Target.Position;
			Steer.DontAvoidance = e => true;
		}
	}

	public override void DoMeleeStrike()
    {
		if (Target == null || Target.LifeState == LifeState.Dead) return;
		if (Target.Position.Distance(Position) < 100)
		{
			MeleeStrike(3, 1.5f);
		}
	}
}
