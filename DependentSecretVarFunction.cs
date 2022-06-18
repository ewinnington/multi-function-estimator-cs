public class DependentSecretVarFunction : ISecretFunction
{
    double eps = 1e-6;
    private double[,] factors;
    private double[,] exponents;

    public int NumberOfFactors;

    int ISecretFunction.NumberOfFactors => NumberOfFactors;
    public DependentSecretVarFunction(int NumberOfFactors)
    {
        Random d = new Random(556873);

        this.NumberOfFactors = NumberOfFactors;
        factors = new double[NumberOfFactors,NumberOfFactors];
        exponents = new double[NumberOfFactors,NumberOfFactors];

        for (int j = 0; j < NumberOfFactors; j++)
        for (int i = 0; i < NumberOfFactors; i++)
        {
                factors[j,i] = (d.NextDouble() - 0.5) * 100;
                exponents[j,i] = (d.NextDouble() - 0.5) * 3;

                if (Math.Abs(exponents[j,i]) < eps)
                {
                    exponents[j,i] = eps;
                }

                if (Math.Abs(factors[j,i]) < eps)
                {
                    factors[j,i] = eps;
                }
        }
    }

    public double ComputeSecretFunction(double[] inputs)
    {
        if (inputs.Length != NumberOfFactors)
        {
            throw new ArgumentException("inputs.Length != NumberOfFactors");
        }

        double result = 0;
        for (int j=0; j<NumberOfFactors; j++)
        {
            double component = 1.0;
            for (int i = 0; i < inputs.Length; i++)
            {
                component *= factors[j,i] * Math.Pow(inputs[i], exponents[j,i]);
            }
            result += component;
        }
        return result;
    }
}