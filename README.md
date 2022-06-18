# Finding the maximum of a secret function by using Parallel For in C#

I create a random function in $i$ dimensions that is represented by the formula

## Independent Variable Secret Function : 
$ \sum_{i}{a_ix_i^{c_i}} $
with $ a_i $ and $ c_i $ randomly drawn. 

$Z$ is a random double variable

Factors $a_i \in \mathbb{R}, a_i = 150(Z - 0.5), Z \in [0..1]$ 

Exponents $c_i \in \mathbb{R}, a_i = 3(Z - 0.2), Z \in [0..1]$

## Dependent Varariable Secret Function : 

$ \sum_{j}{\prod_{i}{a_{i,j}x_i^{c_{i,j}}}} $
with $ a_{i,j} $ and $ c_{i,j} $ randomly drawn. 

$Z$ is a random double variable

Factors $a_i \in \mathbb{R}, a_i = 100(Z - 0.5), Z \in [0..1]$ 

Exponents $c_i \in \mathbb{R}, a_i = 3(Z - 0.5), Z \in [0..1]$

## Brute force search for the maximum using MonteCarlo 

Then using a set of random inputs $x_i$, I try and find the maximum of the function in C# using the Parallel For in the range of $ x_i \in [0..100]$

In this case, I fixed the seed of the secret function to keep it consistent from run to run, so I can check the values. 

Issues encountered and resolved:

1) Random is not thread safe => Create a multi-threaded random

2) Parallel For: I keep a thread local maximum and update the final maximum at the end with a lock to avoid multi-threaded updates to the max value

## Hill climb algorithm 

From a starting position, take a step of 1 unit in the positive and negative direction of each $i$ dimension and obtain the results for each of these possible steps, take the best step and continue with the next step. Exiting when with the gain is too small so as not to loop endlessly due to large step. In case the gain becomes too small, it would be possible to reduce the step sucessively to get closer to the optimal solution. 


To run: 
```
dotnet build -c Release
dotnet run .\bin\Release\net6.0\multi-function-estimator.dll
```
