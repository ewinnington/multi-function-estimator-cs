using System.Diagnostics;

const int NumberOfFactors = 20; 
const double global_max_input = 100; 
const double global_min_input = 0.001;

static double clamp(double x) => Math.Max(global_min_input, Math.Min(x, global_max_input));

IndependentSecretVarFunction secretFunction = new IndependentSecretVarFunction(NumberOfFactors);
//DependentSecretVarFunction secretFunction = new DependentSecretVarFunction(NumberOfFactors);

int maxTries = 10000000;


(double max, int steps, long ms, double[] best_solution) = SolveHillClimb(secretFunction, maxTries);
Console.WriteLine("Max value hill: " + max + " in " + ms + "ms" + " with " + steps + " steps");
Console.WriteLine("Best inputs: " + string.Join("; ", best_solution)); 

SolveHillClimbParallel(secretFunction, maxTries);
SolveParallel(secretFunction, maxTries);
SolveSerial(secretFunction, maxTries);

static void SolveHillClimbParallel(ISecretFunction secretFunction, int maxTries)
{
    const int nClimbers = 50;

    double global_max = double.MinValue;
    object global_max_lock = new object();

    Stopwatch sw = new Stopwatch();
    sw.Start();
    Parallel.For<double>(0, nClimbers, 
    () => { return double.MinValue;}, 
    (i,loop, max_value) => {
        var (max, steps, ms, best_solution) = SolveHillClimb(secretFunction, maxTries, true);
        if(max > max_value)
            max_value = max; 
        return max_value;
    }, (max_value) => {
        lock (global_max_lock)
        {
            if (max_value > global_max)
            {
                global_max = max_value;
            }
        }
    });
    sw.Stop();
    Console.WriteLine("Max value parallel Hill climber: " + global_max + " in " + sw.ElapsedMilliseconds + "ms");
}


static (double max, int steps, long ms, double[] best_solution) SolveHillClimb(ISecretFunction secretFunction, int maxTries, bool StartRandom = false)
{
    int NumberOfFactors = secretFunction.NumberOfFactors; 
    double max = 0;
    double step_size = 1.0; 
    double[] inputs = new double[NumberOfFactors];
    double[] best_inputs = new double[NumberOfFactors];

    // Serial code 
    Stopwatch sw = new Stopwatch();
    sw.Start();
    max = 0;

    //initial position in the middle of the field 
    for (int i = 0; i < NumberOfFactors; i++)
    {
        if(StartRandom) 
            inputs[i] = clamp(MultiThreadRandom.NextDouble() * 100);
        else 
            inputs[i] = inputs[i] = 100/2;
    }

    int step = 0; 
    double gain = 0.1; //exit when it is too small
    while (step < maxTries && gain >= 0.1) {
        double[] results = new double[NumberOfFactors*2+1];
        results[0] = secretFunction.ComputeSecretFunction(inputs);

        for (int d = 0; d < NumberOfFactors; d++)
            {
            double prev = inputs[d];
            inputs[d] = clamp( prev + step_size);  
            results[2*d+1] = secretFunction.ComputeSecretFunction(inputs);
            inputs[d] = clamp(prev - step_size);  
            results[2*d+2] = secretFunction.ComputeSecretFunction(inputs);
            inputs[d] = prev;
            }

        var (best_step, index) = results.Select((n, i) => (n, i)).Max();

        
        gain = best_step - results[0]; 
        max = best_step; 
        
        if(index != 0) {
            int d = ((index+1)/2)-1; 
            if(index % 2 == 1) // odd index
            {
                inputs[d] = clamp( inputs[d] + step_size);
            }
            else // even index
            {
                inputs[d] = clamp( inputs[d] - step_size);
            }
        }
        step++; 
    }
    sw.Stop();
    return (max, step, sw.ElapsedMilliseconds, inputs);
}

static void SolveParallel(ISecretFunction secretFunction, int maxTries)
{
//find the maximum value of the secret function on the range of input 0 to 100.
double max = 0;
object max_lock = new object();

double[] best_inputs = new double[NumberOfFactors];

System.Diagnostics.Stopwatch sw = new Stopwatch();
sw.Start();
//parallelize the tries loop to speed up the process.
Parallel.For<double>(0, maxTries, () => 0, (i,loop, max_sub) =>
    {
        double[] inputs = new double[NumberOfFactors];
        for (int j = 0; j < NumberOfFactors; j++)
        {
            inputs[j] = clamp(MultiThreadRandom.NextDouble() * 100);
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
}

static void SolveSerial(ISecretFunction secretFunction, int maxTries)
{
    int NumberOfFactors = secretFunction.NumberOfFactors; 
    double max = 0;
    double[] inputs = new double[NumberOfFactors];
    double[] best_inputs = new double[NumberOfFactors];

    // Serial code 
    Stopwatch sw = new Stopwatch();
    sw.Start();
    max = 0;
    Random r = new Random();
    for (int tries = 0; tries < maxTries; tries++)
    {
        for (int i = 0; i < NumberOfFactors; i++)
        {
            inputs[i] = clamp((r.NextDouble()) * 100);
        }
        double result = secretFunction.ComputeSecretFunction(inputs);
        if (result > max)
        {
            max = result;
            Array.Copy(inputs,best_inputs,NumberOfFactors);
        }
    }

    Console.WriteLine("Max value serial: " + max + " in " + sw.ElapsedMilliseconds + "ms");
    Console.WriteLine("Best inputs: " + string.Join("; ", best_inputs));
}