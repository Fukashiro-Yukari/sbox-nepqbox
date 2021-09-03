using Sandbox;
using Sandbox.UI;

[Library]
public partial class SandboxHud : HudEntity<RootPanel>
{
	public SandboxHud()
	{
		if ( !IsClient )
			return;

		RootPanel.StyleSheet.Load( "/styles/hud.scss" );

		RootPanel.AddChild<CrosshairCanvas>();
		RootPanel.AddChild<InventoryBar>();
		RootPanel.AddChild<DamageIndicator>();
		RootPanel.AddChild<HitIndicator>();
		RootPanel.AddChild<Ammo>();
		RootPanel.AddChild<Health>();
		RootPanel.AddChild<KillFeed>();
		RootPanel.AddChild<NameTags>();
		RootPanel.AddChild<ChatBox>();
		RootPanel.AddChild<VoiceList>();
		RootPanel.AddChild<Scoreboard<ScoreboardEntry>>();
		RootPanel.AddChild<CurrentTool>();
		RootPanel.AddChild<CurrentFireMode>();
		RootPanel.AddChild<UndoUI>();
		RootPanel.AddChild<SpawnMenu>();
	}
}
