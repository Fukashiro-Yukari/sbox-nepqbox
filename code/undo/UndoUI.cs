using Sandbox;
using Sandbox.UI;

public partial class UndoUI : Panel
{
	public static UndoUI Current;

	public UndoUI()
	{
		Current = this;

		StyleSheet.Load( "/undo/UndoUI.scss" );
	}

	public virtual Panel AddUndoText( string text)
	{
		var e = Current.AddChild<UndoUIEntry>();

		e.Text.Text = $"Undone {text}";

		Sound.FromScreen( "undo" );

		return e;
	}

	public virtual Panel AddCustomUndoText( string text )
	{
		var e = Current.AddChild<UndoUIEntry>();

		e.Text.Text = $"{text}";

		Sound.FromScreen( "undo" );

		return e;
	}
}
