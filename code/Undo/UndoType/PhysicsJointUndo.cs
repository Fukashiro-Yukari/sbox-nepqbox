using Sandbox;

public class PhysicsJointUndo : UndoRemove
{
	IPhysicsJoint Joint;

	public PhysicsJointUndo( IPhysicsJoint j )
	{
		Joint = j;
	}

	public override bool Delete()
	{
		if ( !Joint.IsValid ) return false;

		Joint.Remove();

		return true;
	}

	public override void Replace( object obj )
	{
		var j = obj as IPhysicsJoint;

		if ( j == null ) return;

		Joint = j;
	}

	public override bool ObjectEquals( object obj )
	{
		return Joint.Equals( obj );
	}

	public override bool IsValid()
	{
		return Joint.IsValid;
	}
}
