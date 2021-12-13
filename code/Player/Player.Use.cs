using Sandbox;
using Sandbox.Component;
using System.Linq;

partial class SandboxPlayer
{
	public bool IsUseDisabled()
	{
		return ActiveChild is IUse use && use.IsUsable( this );
	}

	protected override Entity FindUsable()
	{
		if ( IsUseDisabled() )
			return null;

		// First try a direct 0 width line
		var tr = Trace.Ray( EyePos, EyePos + EyeRot.Forward * (85 * Scale) )
			.HitLayer( CollisionLayer.Debris )
			.Ignore( this )
			.Run();

		// See if any of the parent entities are usable if we ain't.
		var ent = tr.Entity;
		while ( ent.IsValid() && !IsValidUseEntity( ent ) )
		{
			ent = ent.Parent;
		}

		// Nothing found, try a wider search
		if ( !IsValidUseEntity( ent ) )
		{
			tr = Trace.Ray( EyePos, EyePos + EyeRot.Forward * (85 * Scale) )
			.Radius( 2 )
			.HitLayer( CollisionLayer.Debris )
			.Ignore( this )
			.Run();

			// See if any of the parent entities are usable if we ain't.
			ent = tr.Entity;
			while ( ent.IsValid() && !IsValidUseEntity( ent ) )
			{
				ent = ent.Parent;
			}
		}

		// Still no good? Bail.
		if ( !IsValidUseEntity( ent ) ) return null;

		return ent;
	}

	protected override void UseFail()
	{
		if ( IsUseDisabled() )
			return;

		base.UseFail();
	}

	ModelEntity lastGlowEntity;

	public virtual void CanUseEntityGlow()
	{
		if ( lastGlowEntity.IsValid() )
		{
			foreach ( var child in lastGlowEntity.Children.OfType<ModelEntity>() )
			{
				if ( child is Player )
					continue;

				if ( child.Components.TryGet<Glow>( out var childglow ) )
				{
					childglow.Active = false;
				}
			}

			if ( lastGlowEntity.Components.TryGet<Glow>( out var glow ) )
			{
				glow.Active = false;
			}

			lastGlowEntity = null;
		}

		var entity = FindUsable();

		if ( entity != null && entity.IsValid && entity is ModelEntity ent )
		{
			lastGlowEntity = ent;

			var glow = ent.Components.GetOrCreate<Glow>();
			glow.Active = true;
			glow.RangeMin = 0;
			glow.RangeMax = 1000;
			glow.Color = new Color( 0.1f, 1.0f, 1.0f, 1.0f );

			foreach ( var child in lastGlowEntity.Children.OfType<ModelEntity>() )
			{
				if ( child is Player )
					continue;

				glow = child.Components.GetOrCreate<Glow>();
				glow.Active = true;
				glow.RangeMin = 0;
				glow.RangeMax = 1000;
				glow.Color = new Color( 0.1f, 1.0f, 1.0f, 1.0f );
			}
		}
	}
}
