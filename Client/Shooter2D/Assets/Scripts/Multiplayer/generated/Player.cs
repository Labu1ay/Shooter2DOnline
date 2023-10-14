// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 1.0.46
// 

using Colyseus.Schema;

public partial class Player : Schema {
	[Type(0, "number")]
	public float pX = default(float);

	[Type(1, "number")]
	public float pY = default(float);

	[Type(2, "number")]
	public float pZ = default(float);

	[Type(3, "number")]
	public float vX = default(float);

	[Type(4, "number")]
	public float vY = default(float);

	[Type(5, "number")]
	public float vZ = default(float);

	[Type(6, "number")]
	public float rY = default(float);

	[Type(7, "int8")]
	public sbyte maxHP = default(sbyte);

	[Type(8, "int8")]
	public sbyte currentHP = default(sbyte);

	[Type(9, "uint16")]
	public ushort score = default(ushort);
}

