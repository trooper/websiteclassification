namespace PsiMl.WebsiteClasification
{
    using Accord.MachineLearning.Bayes;
    using Accord.Statistics.Models.Regression;
    using Accord.Statistics.Models.Regression.Fitting;
    using System;

    public class SomeExamples
    {
        public static void NaiveBayes()
        {
            int[][] inputs =
             {
                 //               input      output
                 new int[] { 0, 1, 1, 0 }, //  0 
                 new int[] { 0, 1, 0, 0 }, //  0
                 new int[] { 0, 0, 1, 0 }, //  0
                 new int[] { 0, 1, 1, 0 }, //  0
                 new int[] { 0, 1, 0, 0 }, //  0
                 new int[] { 1, 0, 0, 0 }, //  1
                 new int[] { 1, 0, 0, 0 }, //  1
                 new int[] { 1, 0, 0, 1 }, //  1
                 new int[] { 0, 0, 0, 1 }, //  1
                 new int[] { 0, 0, 0, 1 }, //  1
                 new int[] { 1, 1, 1, 1 }, //  2
                 new int[] { 1, 0, 1, 1 }, //  2
                 new int[] { 1, 1, 0, 1 }, //  2
                 new int[] { 0, 1, 1, 1 }, //  2
                 new int[] { 1, 1, 1, 1 }, //  2
             };

            int[] outputs = // those are the class labels
             {
                 0, 0, 0, 0, 0,
                 1, 1, 1, 1, 1,
                 2, 2, 2, 2, 2,
             };

            // Create a discrete naive Bayes model for 3 classes and 4 binary inputs
            var bayes = new NaiveBayes(classes: 3, symbols: new int[] { 2, 2, 2, 2 });

            // Teach the model. The error should be zero:
            double error = bayes.Estimate(inputs, outputs);

            // Now, let's test  the model output for the first input sample:
            int answer = bayes.Compute(new int[] { 1, 1, 1, 1 }); // should be 1
        }

        public static void LogisticRegression()
        {
            double[][] inputs = new double[14][];
            inputs[0] = new double[] { 0, 0, 0, 0 };
            inputs[1] = new double[] { 0, 0, 0, 1 };
            inputs[2] = new double[] { 1, 0, 0, 0 };
            inputs[3] = new double[] { 2, 1, 0, 0 };
            inputs[4] = new double[] { 2, 2, 1, 0 };
            inputs[5] = new double[] { 2, 2, 1, 1 };
            inputs[6] = new double[] { 1, 2, 1, 1 };
            inputs[7] = new double[] { 0, 1, 0, 0 };
            inputs[8] = new double[] { 0, 2, 1, 0 };
            inputs[9] = new double[] { 2, 1, 1, 0 };
            inputs[10] = new double[] { 0, 1, 1, 1 };
            inputs[11] = new double[] { 1, 1, 0, 1 };
            inputs[12] = new double[] { 1, 0, 1, 0 };
            inputs[13] = new double[] { 2, 1, 0, 1 };

            double[] outputs = { 0, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 1, 1, 0 };

            var regression = new Accord.Statistics.Models.Regression.LogisticRegression(inputs: 4);

            var teacher = new Accord.Statistics.Models.Regression.Fitting.LogisticGradientDescent(regression);
            teacher.LearningRate = 0.01;

            double delta = 0;
            do
            {
                delta = teacher.Run(inputs, outputs);
            } while (delta > 0.001);

            var input = new double[] { 0, 0, 0, 0 };
            var result = regression.Compute(input);
            Console.WriteLine(result);

            input = new double[] { 1, 0, 0, 0 };
            result = regression.Compute(input);
            Console.WriteLine(result);

            Console.ReadLine();
        }

        public static void MulticlassLogisticRegression()
        {
            double[][] inputs = new double[14][];
            inputs[0] = new double[] { 0, 0, 0, 0 };
            inputs[1] = new double[] { 0, 0, 0, 1 };
            inputs[2] = new double[] { 1, 0, 0, 0 };
            inputs[3] = new double[] { 2, 1, 0, 0 };
            inputs[4] = new double[] { 2, 2, 1, 0 };
            inputs[5] = new double[] { 2, 2, 1, 1 };
            inputs[6] = new double[] { 1, 2, 1, 1 };
            inputs[7] = new double[] { 0, 1, 0, 0 };
            inputs[8] = new double[] { 0, 2, 1, 0 };
            inputs[9] = new double[] { 2, 1, 1, 0 };
            inputs[10] = new double[] { 0, 1, 1, 1 };
            inputs[11] = new double[] { 1, 1, 0, 1 };
            inputs[12] = new double[] { 1, 0, 1, 0 };
            inputs[13] = new double[] { 2, 1, 0, 1 };

            int[] outputs = { 0, 0, 1, 1, 2, 2, 1, 1, 0, 2, 0, 1, 1, 2 };

            var regression = new MultinomialLogisticRegression(inputs: 4, categories: 3);
            LowerBoundNewtonRaphson lbnr = new LowerBoundNewtonRaphson(regression);

            double delta;
            int iteration = 0;

            do
            {
                // Perform an iteration
                delta = lbnr.Run(inputs, outputs);
                iteration++;
            } while (iteration < 100 && delta > 1e-6);

            var input = new double[] { 0, 0, 0, 0 };
            var result = regression.Compute(input);
            Console.WriteLine(string.Join(" ", result));

            input = new double[] { 1, 0, 0, 0 };
            result = regression.Compute(input);
            Console.WriteLine(string.Join(" ", result));

            var data = regression.Coefficients;
            regression = null;
            regression = new MultinomialLogisticRegression(4, 3);
            regression.Coefficients = data;

            input = new double[] { 0, 0, 0, 0 };
            result = regression.Compute(input);
            Console.WriteLine(string.Join(" ", result));

            input = new double[] { 1, 0, 0, 0 };
            result = regression.Compute(input);
            Console.WriteLine(string.Join(" ", result));


            Console.ReadLine();
        }
    }
}
