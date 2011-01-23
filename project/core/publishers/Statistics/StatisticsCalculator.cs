using System;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
    /// <summary>
    /// 	
    /// </summary>
	public class StatisticsCalculator
	{
        /// <summary>
        /// Variances the specified list.	
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public double Variance(double[] list)
		{
			try
			{
				double s = 0;
				double mean = Mean(list);
				for (int i = 0; i <= list.Length - 1; i++)
				{
					s += Math.Pow(list[i] - mean, 2);
				}
				return s/list.Length;
			}
			catch (Exception)
			{
				return double.NaN;
			}
		}

        /// <summary>
        /// Means the specified list.	
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public double Mean(double[] list)
		{
			try
			{
				double sum = 0;
				for (int i = 0; i <= list.Length - 1; i++)
				{
					sum += list[i];
				}
				return sum/list.Length;
			}
			catch (Exception)
			{
				return double.NaN;
			}
		}

        /// <summary>
        /// Standards the deviation.	
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public double StandardDeviation(double[] list)
		{
			return Math.Sqrt(Variance(list));
		}
	}
}