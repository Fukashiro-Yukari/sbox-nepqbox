using Sandbox;

partial class NPC
{
	ModelEntity pants;
	ModelEntity jacket;
	ModelEntity shoes;
	ModelEntity hat;

	public void Dress()
	{
		if ( true )
		{
			var model = Rand.FromArray( new[]
			{
				"models/citizen_clothes/trousers/trousers.jeans.vmdl",
				"models/citizen_clothes/trousers/trousers.lab.vmdl",
				"models/citizen_clothes/trousers/trousers.police.vmdl",
				"models/citizen_clothes/trousers/trousers.smart.vmdl",
				"models/citizen_clothes/trousers/trousers.smarttan.vmdl",
				"models/citizen_clothes/trousers/trousers_tracksuitblue.vmdl",
				"models/citizen_clothes/trousers/trousers_tracksuit.vmdl",
				"models/citizen_clothes/shoes/shorts.cargo.vmdl",
				"models/citizen_clothes/dress/dress.kneelength.vmdl"
			} );

			pants = new ModelEntity();
			pants.SetModel( model );
			pants.SetParent( this, true );
			pants.EnableShadowInFirstPerson = true;
			pants.EnableHideInFirstPerson = true;
			pants.Tags.Add( "clothes" );

			SetBodyGroup( "Legs", 1 );

			if ( model.Contains( "dress" ) )
			{
				SetBodyGroup( "Legs", 0 );

				jacket = pants;
			}
		}

		if ( true && jacket == null )
		{
			var model = Rand.FromArray( new[]
			{
				"models/citizen_clothes/jacket/labcoat.vmdl",
				"models/citizen_clothes/jacket/jacket.red.vmdl",
				"models/citizen_clothes/jacket/jacket.tuxedo.vmdl",
				"models/citizen_clothes/jacket/jacket_heavy.vmdl",
			} );

			jacket = new ModelEntity();
			jacket.SetModel( model );
			jacket.SetParent( this, true );
			jacket.EnableShadowInFirstPerson = true;
			jacket.EnableHideInFirstPerson = true;
			jacket.Tags.Add( "clothes" );

			//var propInfo = jacket.Model.GetPropData();

			//if ( propInfo?.ParentBodygroupName != null )
			//{
			//	SetBodyGroup( propInfo.ParentBodygroupName, propInfo.ParentBodygroupValue );
			//}
			//else
			//{
				SetBodyGroup( "Chest", 0 );
			//}
		}

		if ( true )
		{
			var model = Rand.FromArray( new[]
			{
				"models/citizen_clothes/shoes/trainers.vmdl",
				"models/citizen_clothes/shoes/shoes.workboots.vmdl"
			} );

			shoes = new ModelEntity();
			shoes.SetModel( model );
			shoes.SetParent( this, true );
			shoes.EnableShadowInFirstPerson = true;
			shoes.EnableHideInFirstPerson = true;
			shoes.Tags.Add( "clothes" );

			SetBodyGroup( "Feet", 1 );
		}

		if ( true )
		{
			var model = Rand.FromArray( new[]
			{
				"models/citizen_clothes/hat/hat_hardhat.vmdl",
				"models/citizen_clothes/hat/hat_woolly.vmdl",
				"models/citizen_clothes/hat/hat_securityhelmet.vmdl",
				"models/citizen_clothes/hair/hair_malestyle02.vmdl",
				"models/citizen_clothes/hair/hair_femalebun.black.vmdl",
				"models/citizen_clothes/hat/hat_beret.red.vmdl",
				"models/citizen_clothes/hat/hat.tophat.vmdl",
				"models/citizen_clothes/hat/hat_beret.black.vmdl",
				"models/citizen_clothes/hat/hat_cap.vmdl",
				"models/citizen_clothes/hat/hat_leathercap.vmdl",
				"models/citizen_clothes/hat/hat_leathercapnobadge.vmdl",
				"models/citizen_clothes/hat/hat_securityhelmetnostrap.vmdl",
				"models/citizen_clothes/hat/hat_service.vmdl",
				"models/citizen_clothes/hat/hat_uniform.police.vmdl",
				"models/citizen_clothes/hat/hat_woollybobble.vmdl",
			} );

			hat = new ModelEntity();
			hat.SetModel( model );
			hat.SetParent( this, true );
			hat.EnableShadowInFirstPerson = true;
			hat.EnableHideInFirstPerson = true;
			hat.Tags.Add( "clothes" );
		}
	}

	public void DeleteAllDress()
	{
		while ( Children.Count > 0 )
		{
			for ( var i = 0; i < Children.Count; i++ )
			{
				var child = Children[i];

				if ( !child.Tags.Has( "clothes" ) ) continue;
				if ( child is not ModelEntity ) continue;

				child.Delete();
			}
		}

		SetBodyGroup( "head", 0 );
		SetBodyGroup( "Chest", 0 );
		SetBodyGroup( "Legs", 0 );
		SetBodyGroup( "Hands", 0 );
		SetBodyGroup( "Feet", 0 );
	}

	public void AddClothing( string model )
	{
		new ModelEntity( model, this ).Tags.Add( "clothes" );
	}
}
