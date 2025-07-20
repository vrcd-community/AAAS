namespace AAAS.Slide.Tools {
    public static class MathTools {
        /// <summary>
        /// Returns value clamped to the inclusive range of min and max.
        /// </summary>
        /// <param name="value">The value to be clamped.</param>
        /// <param name="min">The lower bound of the result.</param>
        /// <param name="max">The upper bound of the result.</param>
        /// <returns>Return NaN If min > max</returns>
        public static double Clamp(double value, double min, double max)
        {
            if (min > max) {
                return double.NaN;
            }

            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }
    }
}
