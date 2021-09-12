using Sandbox;

public class ColorUndo : UndoRemove
{
	ModelEntity entity;
	Color color;

	public ColorUndo( ModelEntity ent, Color col )
	{
		entity = ent;
		color = col;
	}

	public override bool Delete()
	{
		if ( !entity.IsValid() || entity.IsWorld ) return false;

		entity.RenderColor = color;

		return true;
	}

	public override void Replace( object obj )
	{
		if ( !(obj is Color c) ) return;

		color = c;
	}

	public override bool ObjectEquals( object obj )
	{
		return color.Equals( obj );
	}

	public override bool IsValid()
	{
		return entity.IsValid();
	}
}
