using UnityEngine;
using System;

public class GrowthRate : MonoBehaviour
{
    Parameters parameters;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Computed Growth Data " + ComputeGrowthRate(parameters));
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static double GetPrefactor(double N0, double v, double omega, double C0, double QD, double k, double T, double C_C0)
    {
        // np.log -> Math.Log (natural log)
        return N0 * v * omega * C0 * Math.Exp(-QD) * C_C0 * Math.Sqrt(Math.Log(C_C0));
    }

    public static double GetNucleationBarrier(double x, double a, double k, double T, double C_C0)
    {
        double numerator = -Math.PI * Math.Pow((x * a) / (k * T), 2.0);
        double denominator = Math.Log(C_C0); // natural log
        return Math.Exp(numerator / denominator);
    }

    public static double GetNucleationFrequencyPerUnitArea(Parameters p)
    {
        double prefactor = GetPrefactor(
            p.N0_number_of_atomic_sites,
            p.v_vibrational_frequency,
            p.omega_atomic_volume,
            p.C0,
            p.QD_activation_energy,
            p.k_boltzmann_constant,
            p.T_temperature,
            p.C_C0_supersaturation
        );

        double barrier = GetNucleationBarrier(
            p.x_edge_energy,
            p.a_automic_size,
            p.k_boltzmann_constant,
            p.T_temperature,
            p.C_C0_supersaturation
        );

        return prefactor * barrier;
    }

    public static double ComputeGrowthRate(Parameters p)
    {
        // Mirror the Python: R = J * π * r^2 * a
        // Note: r_radius is given in "nm" per your comment; Python used it as-is.
        double J = GetNucleationFrequencyPerUnitArea(p);
        return J * Math.PI * Math.Pow(p.r_radius, 2.0) * p.a_automic_size;
    }
}

public class Parameters
    {
        // Default values — replace with yours
        public double N0_number_of_atomic_sites = 1e19;     // m^-2
        public double v_vibrational_frequency = 1e13;       // s^-1
        public double omega_atomic_volume = 2e-29;          // m^3/atom
        public double QD_activation_energy = 5.0;           // dimensionless "in kT" if that's what you intend
        public double T_temperature = 900.0;                // Kelvin if that's what you intend
        public double C_C0_supersaturation = 1.2;           // C/C0 (must be > 1)
        public double x_edge_energy = 1e-10;                // J m^-1
        public double a_automic_size = 2.7e-10;             // m
        public double k_boltzmann_constant = 1.380649e-23;  // J/K
        public double r_radius = 10.0;                      // nm (note: your Python uses this as-is)
        public double C0 = 1.0;                             // baseline concentration (set appropriately)
    }
