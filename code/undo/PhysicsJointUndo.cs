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

	public override bool Replace( object obj )
	{
		var j = obj as IPhysicsJoint;

		if ( j == null ) return false;

		Joint = j;

		return true;
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
