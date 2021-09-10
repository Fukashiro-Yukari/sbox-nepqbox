using Sandbox;

[Library( "npc_dummy", Title = "Dummy", Spawnable = true )]
public partial class NpcDummy : Npc
{
	public override float SpawnHealth => 100;

	[Event.Frame]
	public void OnFrame()
	{
		DebugOverlay.Text( EyePos, Health.ToString() );
	}
}
