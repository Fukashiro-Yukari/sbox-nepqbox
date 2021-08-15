using Sandbox;
using System;

[Library("weapon_crowbar", Title = "Crowbar", Spawnable = true)]
[Hammer.EditorModel("weapons/rust_boneknife/rust_boneknife.vmdl")]
partial class Crowbar : Weapon
{
	public override string ViewModelPath => "models/weapons/v_crowbar.vmdl";

	public override int ClipSize => -1;
	public override float PrimaryRate => 4.0f;
	public override float SecondaryRate => 0.5f;
	public override float ReloadTime => 0f;
	public override int Bucket => 0;
	public override CType Crosshair => CType.None;
	public virtual int BaseDamage => 25;
	public virtual int MeleeDistance => 90;

	public override void Spawn()
	{
		base.Spawn();

		SetModel("weapons/rust_pistol/rust_pistol.vmdl");
	}

	bool isFlesh;

	private bool MeleeAttack()
	{
		var forward = Owner.EyeRot.Forward;
		forward = forward.Normal;

		bool hit = false;

		foreach (var tr in TraceBullet(Owner.EyePos, Owner.EyePos + forward * MeleeDistance, 5.0f))
		{
			if (!tr.Entity.IsValid()) continue;

			tr.Surface.DoBulletImpact(tr);

			hit = true;
			isFlesh = tr.Entity is SandboxPlayer || tr.Entity is Npc;

			if (!IsServer) continue;

			using (Prediction.Off())
			{
				var damageInfo = DamageInfo.FromBullet(tr.EndPos, forward * 100, BaseDamage)
					.UsingTraceResult(tr)
					.WithAttacker(Owner)
					.WithWeapon(this);

				tr.Entity.TakeDamage(damageInfo);
			}
		}

		return hit;
	}

	public override void AttackPrimary()
	{
		if (!BaseAttackPrimary()) return;
		if (MeleeAttack())
		{
			PlaySound(isFlesh ? "weapon_crowbar.hit": "weapon_crowbar.hitworld");

			OnMeleeHit();
		}
		else
		{
			PlaySound("weapon_crowbar.swing");

			OnMeleeMiss();
		}
	}

	[ClientRpc]
	private void OnMeleeMiss()
	{
		Host.AssertClient();

		ViewModelEntity?.SetAnimBool("fire_miss", true);
	}

	[ClientRpc]
	private void OnMeleeHit()
	{
		Host.AssertClient();

		if (IsLocalPawn)
		{
			_ = new Sandbox.ScreenShake.Perlin(1.0f, 1.0f, 3.0f);
		}

		ViewModelEntity?.SetAnimBool("fire", true);
	}

	public override void SimulateAnimator(PawnAnimator anim)
	{
		anim.SetParam("holdtype", 4); // TODO this is shit
		anim.SetParam("aimat_weight", 1.0f);
	}
}
