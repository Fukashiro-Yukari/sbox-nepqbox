using Sandbox;

public partial class Carriable : BaseCarriable, IUse
{
	public virtual int Bucket => 1;
	public virtual int BucketWeight => 100;
	public virtual string WorldModelPath => "";
	public virtual string Icon => "";
	public virtual string DrawAnim => "deploy";
	public virtual string DrawEmptyAnim => null;

	public override void Spawn()
	{
		base.Spawn();

		if ( !string.IsNullOrEmpty( WorldModelPath ) )
			SetModel( WorldModelPath );
	}

	/// <summary>
	/// This entity has become the active entity. This most likely
	/// means a player was carrying it in their inventory and now
	/// has it in their hands.
	/// </summary>
	public override void ActiveStart( Entity ent )
	{
		//base.ActiveStart( ent );

		EnableDrawing = true;

		if ( ent is Player player )
		{
			var animator = player.GetActiveAnimator();
			if ( animator != null )
			{
				SimulateAnimator( animator );
			}
		}

		//
		// If we're the local player (clientside) create viewmodel
		// and any HUD elements that this weapon wants
		//
		if ( IsLocalPawn )
		{
			DestroyViewModel();
			DestroyHudElements();

			CreateViewModel();
			CreateHudElements();

			if ( this is Weapon wep )
			{
				if ( wep.AmmoClip <= 0 && !string.IsNullOrEmpty( DrawEmptyAnim ) )
					ViewModelEntity?.SetAnimBool( DrawEmptyAnim, true );
				else if ( !string.IsNullOrEmpty( DrawAnim ) )
					ViewModelEntity?.SetAnimBool( DrawAnim, true );
			}
			else if ( !string.IsNullOrEmpty( DrawAnim ) )
				ViewModelEntity?.SetAnimBool( DrawAnim, true );
		}
	}

	public override void CreateViewModel()
	{
		Host.AssertClient();

		if ( string.IsNullOrEmpty( ViewModelPath ) )
			return;

		ViewModelEntity = new ViewModel
		{
			Position = Position,
			Owner = Owner,
			EnableViewmodelRendering = true
		};

		ViewModelEntity.SetModel( ViewModelPath );
	}

	public virtual bool OnUse( Entity user )
	{
		return false;
	}

	public virtual bool IsUsable( Entity user )
	{
		return Owner == null;
	}
}
