public class IndependentSecretVarFunction
{
    double eps = 1e-6;
    private double[] factors;
    private double[] exponents;

    public int NumberOfFactors; 

    public IndependentSecretVarFunction(int NumberOfFactors)
    {
        Random d = new Random(556872);

        this.NumberOfFactors = NumberOfFactors;
        factors = new double[NumberOfFactors];
        exponents = new double[NumberOfFactors];

        for (int i = 0; i < NumberOfFactors; i++)
        {
            factors[i] = (d.NextDouble() - 0.5) * 150;
            exponents[i] = (d.NextDouble() - 0.2) * 3;

            if (Math.Abs(exponents[i]) < eps)
            {
                exponents[i] = eps;
            }

            if (Math.Abs(factors[i]) < eps)
            {
                factors[i] = eps;
            }
        }

        //Console.WriteLine("Secret function:" + string.Join("; ", factors) + "^" + string.Join("; ", exponents));
    }

    public double ComputeSecretFunction(double[] inputs)
    {
        if (inputs.Length != NumberOfFactors)
        {
            throw new ArgumentException("inputs.Length != NumberOfFactors");
        }

        double result = 0;
        for (int i = 0; i < inputs.Length; i++)
        {
            result += factors[i] * Math.Pow(inputs[i], exponents[i]);
        }
        return result;
    }
}