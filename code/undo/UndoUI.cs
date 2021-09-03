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

	public virtual Panel AddEntry( string text)
	{
		var e = Current.AddChild<UndoUIEntry>();

		e.Text.Text = $"Undo {text}";

		Sound.FromScreen( "undo" );

		return e;
	}
}
