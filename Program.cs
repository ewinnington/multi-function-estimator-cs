using System.Diagnostics;

const int NumberOfFactors = 3; 
IndependentSecretVarFunction secretFunction = new IndependentSecretVarFunction(NumberOfFactors);
//DependentSecretVarFunction secretFunction = new DependentSecretVarFunction(NumberOfFactors);

//find the maximum value of the secret function on the range of input 0 to 100.
double max = 0;
object max_lock = new object();
int maxTries = 10000000;

double[] best_inputs = new double[NumberOfFactors];

System.Diagnostics.Stopwatch sw = new Stopwatch();
sw.Start();
//parallelize the tries loop to speed up the process.
Parallel.For<double>(0, maxTries, () => 0, (i,loop, max_sub) =>
    {
        double[] inputs = new double[NumberOfFactors];
        for (int j = 0; j < NumberOfFactors; j++)
        {
            inputs[j] = MultiThreadRandom.NextDouble() * 100;
        }
        double result = secretFunction.ComputeSecretFunction(inputs);
        if(result > max_sub)
            max_sub = result;
        return max_sub; 
    }, 
    (max_sub) => {
        //Console.WriteLine("maxsub: " + max_sub);
        lock(max_lock) {
            if (max_sub > max)
            {
                max = max_sub;
            }
        };
    });
sw.Stop();

Console.WriteLine("Max value parallel: " + max + " in " + sw.ElapsedMilliseconds + "ms");
// Console.WriteLine("Best inputs: " + string.Join("; ", best_inputs)); //Not saving the best inputs since I'm not interested in the result.

// Serial code 
sw.Reset();
sw.Start();
max = 0;
Random r = new Random();
for (int tries = 0; tries < maxTries; tries++)
{
    double[] inputs = new double[NumberOfFactors];
    for (int i = 0; i < NumberOfFactors; i++)
    {
        inputs[i] = (r.NextDouble()) * 100;
    }
    double result = secretFunction.ComputeSecretFunction(inputs);
    if (result > max)
    {
        max = result;
        Array.Copy(inputs,best_inputs,NumberOfFactors);
    }
/*
    if(tries % 100000 == 0)
    {
        Console.WriteLine("Tries: " + tries + " Max: " + max);
    }
    */
}

Console.WriteLine("Max value serial: " + max + " in " + sw.ElapsedMilliseconds + "ms");
Console.WriteLine("Best inputs: " + string.Join("; ", best_inputs));
