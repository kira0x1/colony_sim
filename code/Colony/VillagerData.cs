namespace Kira;

using System;

// ReSharper disable ConvertConstructorToMemberInitializers
public class VillagerData
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }

    /// <summary>
    /// Height in cm
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Weight in kg
    /// </summary>
    public int Weight { get; set; }

    public VillagerData()
    {
        FirstName = RandomNames.RandomFirstName;
        LastName = RandomNames.RandomLastName;

        Age = Random.Shared.Int(10, 60);
        Weight = Random.Shared.Int(50, 120);

        Height = Utils.VillagerUtils.RandomHeight(Age);
        // Age = VillagerUtils.RandomAge;
        // Log.Info(this);
    }

    public override string ToString()
    {
        return $"Name: {FirstName} {LastName}, Age: {Age}, Height: {Height}cm, Weight: {Weight}kg";
    }
}