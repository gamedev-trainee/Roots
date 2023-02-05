namespace Roots
{
    public class CoreUtils
    {
        public static void FilterRotation(ref float value)
        {
            while (value < 0f) value += 360f;
            while (value > 360f) value -= 360f;
        }

        public static bool FilterRotation(ref float start, ref float end)
        {
            FilterRotation(ref start);
            FilterRotation(ref end);
            if (start < end)
            {
                FilterRotationPair(ref start, ref end);
                return true;
            }
            else if (start > end)
            {
                FilterRotationPair(ref end, ref start);
                return true;
            }
            return false;
        }

        protected static void FilterRotationPair(ref float min, ref float max)
        {
            float diff = max - min;
            float diffElse = (min - 0f) + (360f - max);
            if (diffElse < diff)
            {
                min += 360f;
            }
        }

        public static float GetRotationDiff(float rotationA, float rotationB)
        {
            FilterRotation(ref rotationA);
            FilterRotation(ref rotationB);
            if (rotationA < rotationB)
            {
                return GetRotationPairDiff(rotationA, rotationB);
            }
            else if (rotationA > rotationB)
            {
                return GetRotationPairDiff(rotationB, rotationA);
            }
            return 0f;
        }

        protected static float GetRotationPairDiff(float min, float max)
        {
            float diff = max - min;
            float diffElse = (min - 0f) + (360f - max);
            if (diffElse < diff)
            {
                return diffElse;
            }
            return diff;
        }
    }
}
