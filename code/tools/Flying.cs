using Sandbox;


[Library("weapon_flying", Title = "Flying", Spawnable = true )]
[Hammer.EditorModel( "weapons/rust_pistol/rust_pistol.vmdl" )]
partial class Flying : Carriable
{ 
	public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";
	public override int Bucket => 8;
	private PawnController LastController;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/rust_pistol/rust_pistol.vmdl" );
	}

	private void Activate()
    {
		if (Owner is not Player owner) return;

		LastController = owner.Controller;
		owner.Controller = new SandboxFlyingController();
	}

	private void Deactivate()
    {
		if (Owner is not Player owner) return;

		owner.Controller = LastController;
	}

	public override void ActiveStart(Entity ent)
	{
		base.ActiveStart(ent);

		Activate();
	}

	public override void ActiveEnd(Entity ent, bool dropped)
	{
		base.ActiveEnd(ent, dropped);

		Deactivate();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		Deactivate();
	}

	public override void OnCarryDrop(Entity dropper)
	{
		base.OnCarryDrop(dropper);

		Deactivate();
	}
}
