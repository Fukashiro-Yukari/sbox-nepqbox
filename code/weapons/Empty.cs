using Sandbox;


[Library("weapon_empty", Title = "Empty", Spawnable = true )]
[Hammer.EditorModel( "weapons/rust_pistol/rust_pistol.vmdl" )]
partial class Empty : Carriable
{ 
	public override string ViewModelPath => "";
	public override int Bucket => 0;

    public override void OnCarryDrop(Entity dropper)
	{
	}

	public override void CreateHudElements()
    {
    }

	public override void SimulateAnimator(PawnAnimator anim)
	{
		anim.SetParam("holdtype", 0); // TODO this is shit
		anim.SetParam("aimat_weight", 1.0f);
	}
}
