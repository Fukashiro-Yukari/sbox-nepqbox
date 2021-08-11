using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class ClipAmmo : Panel
{
	public Label Label;

	public ClipAmmo()
	{
		Label = Add.Label( "- / -", "value" );
	}

	private void Clear()
	{
		Label.Text = "- / -";

		SetClass( "close", true );
	}

	public override void Tick()
	{
		var player = Local.Pawn;

		SetClass( "close", false );

		if ( player == null )
		{
			Clear();
			return;
		}

		var wep = player.ActiveChild as Weapon;

		if ( wep == null )
		{
			Clear();
			return;
		}

		var clipsize = wep.ClipSize;
		var clipammo = wep.AmmoClip;

		if ( clipsize < 0 )
		{
			Clear();
			return;
		}

		if ( clipammo > clipsize )
			Label.Text = $"{clipammo - (clipammo - clipsize)} + {clipammo - clipsize} / {clipsize}";
		else
			Label.Text = $"{clipammo} / {clipsize}";

		var clipcon = (float)clipammo / (float)clipsize;

		//Log.Info( clipcon - 1 );

		//Style.FontColor = Color.White;
		//Style.FontColor = Color.Lerp( Color.White, Color.Yellow, clipcon );

		if ( clipcon < 0.5 && clipcon > 0.2 )
		{
			SetClass( "yellow", true );
			SetClass( "danger", false );
		}
		else if ( clipcon < 0.2 )
		{
			SetClass( "yellow", false );
			SetClass( "danger", true );
		}
		else
		{
			SetClass( "yellow", false );
			SetClass( "danger", false );
		}
	}
}
