using Sandbox;
using System;

[Spawnable]
[Library( "ent_meloncar", Title = "Melon Car" )]
public partial class MelonCar : CarEntity
{
	public override float Height => 30f;
	public override float LeanForward => 10f;
	public override float LeanRight => 10f;

	public override void ClientCreateModel()
	{
		{
			var vehicle_fuel_tank = new ModelEntity();
			vehicle_fuel_tank.SetModel( "entities/modular_vehicle/vehicle_fuel_tank.vmdl" );
			vehicle_fuel_tank.Transform = Transform;
			vehicle_fuel_tank.Parent = this;
			vehicle_fuel_tank.LocalPosition = new Vector3( 0.75f, 0, 0 ) * 40.0f;
		}

		{
			chassis_axle_front = new ModelEntity();
			chassis_axle_front.SetModel( "entities/modular_vehicle/chassis_axle_front.vmdl" );
			chassis_axle_front.Transform = Transform;
			chassis_axle_front.Parent = this;
			chassis_axle_front.LocalPosition = new Vector3( 1.05f, 0, 0.35f ) * 40.0f;

			{
				wheel0 = new ModelEntity();
				wheel0.SetModel( "models/sbox_props/watermelon/watermelon.vmdl" );
				wheel0.SetParent( chassis_axle_front, "Wheel_Steer_R", new Transform( Vector3.Zero, Rotation.From( 0, 180, 0 ) ) );
			}

			{
				wheel1 = new ModelEntity();
				wheel1.SetModel( "models/sbox_props/watermelon/watermelon.vmdl" );
				wheel1.SetParent( chassis_axle_front, "Wheel_Steer_L", new Transform( Vector3.Zero, Rotation.From( 0, 0, 0 ) ) );
			}

			{
				var chassis_steering = new ModelEntity();
				chassis_steering.SetModel( "entities/modular_vehicle/chassis_steering.vmdl" );
				chassis_steering.SetParent( chassis_axle_front, "Axle_front_Center", new Transform( Vector3.Zero, Rotation.From( -90, 180, 0 ) ) );
			}
		}

		{
			chassis_axle_rear = new ModelEntity();
			chassis_axle_rear.SetModel( "entities/modular_vehicle/chassis_axle_rear.vmdl" );
			chassis_axle_rear.Transform = Transform;
			chassis_axle_rear.Parent = this;
			chassis_axle_rear.LocalPosition = new Vector3( -1.05f, 0, 0.35f ) * 40.0f;

			{
				var chassis_transmission = new ModelEntity();
				chassis_transmission.SetModel( "entities/modular_vehicle/chassis_transmission.vmdl" );
				chassis_transmission.SetParent( chassis_axle_rear, "Axle_Rear_Center", new Transform( Vector3.Zero, Rotation.From( -90, 180, 0 ) ) );
			}

			{
				wheel2 = new ModelEntity();
				wheel2.SetModel( "models/sbox_props/watermelon/watermelon" );
				wheel2.SetParent( chassis_axle_rear, "Axle_Rear_Center", new Transform( Vector3.Left * (0.7f * 40) + Vector3.Down * 5, Rotation.From( 0, 90, 0 ) ) );
			}

			{
				wheel3 = new ModelEntity();
				wheel3.SetModel( "models/sbox_props/watermelon/watermelon" );
				wheel3.SetParent( chassis_axle_rear, "Axle_Rear_Center", new Transform( Vector3.Right * (0.7f * 40), Rotation.From( 0, -90, 0 ) ) );
			}
		}
	}

    float wheelAngle = 0.0f;
    float wheelRevolute = 0.0f;

    public override void OnCarFrame()
    {
        wheelAngle = wheelAngle.LerpTo(TurnDirection * 25, 1.0f - MathF.Pow(0.001f, Time.Delta));
        wheelRevolute += (WheelSpeed / (14.0f * Scale)).RadianToDegree() * Time.Delta;

        var wheelRotRight = Rotation.From(-wheelAngle, 90, -wheelRevolute);
        var wheelRotLeft = Rotation.From(wheelAngle, 90, wheelRevolute);
        var wheelRotBackRight = Rotation.From(0, 180, wheelRevolute);
        var wheelRotBackLeft = Rotation.From(0, -180, -wheelRevolute);

        RaycastWheels(Rotation, false, out _, out _, Time.Delta);

        float frontOffset = 20.0f - Math.Min(frontLeftDistance, frontRightDistance);
        float backOffset = 20.0f - Math.Min(backLeftDistance, backRightDistance);

        chassis_axle_front.SetBoneTransform("Axle_front_Center", new Transform(Vector3.Up * frontOffset), false);
        chassis_axle_rear.SetBoneTransform("Axle_Rear_Center", new Transform(Vector3.Up * backOffset), false);

        wheel0.LocalRotation = wheelRotRight;
        wheel1.LocalRotation = wheelRotLeft;
        wheel2.LocalRotation = wheelRotBackRight;
        wheel3.LocalRotation = wheelRotBackLeft;
    }
}
