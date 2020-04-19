using System;
using UnityEngine;

public class FiringSolution
{
    public enum Conditions
    {
        UserSelected,
        Fallback,
    }

    public Vector3 InitialVelocity { get; set; }
    public Conditions Condition { get; set; }
}

public interface IReportFiringSolutions
{
    event Action<FiringSolution> FiringSolutionFound;
}
