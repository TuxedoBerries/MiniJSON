using System;

namespace MiniJSON
{
	public static class JSONHelper
	{
		#region Numbers
		public static short ConvertToShort(object obj)
		{
			if (obj is long)
				return (short)((long)obj);

			if(obj is double)
				return (short)((double)obj);

			return 0;
		}

		public static int ConvertToInt(object obj)
		{
			if (obj is long)
				return (int)((long)obj);

			if(obj is double)
				return (int)((double)obj);
			
			return 0;
		}

		public static long ConvertToLong(object obj)
		{
			if (obj is long)
				return (long)obj;

			if(obj is double)
				return (long)((double)obj);

			return 0;
		}

		public static float ConvertToFloat(object obj)
		{
			if (obj is double)
				return (float)((double)obj);

			return 0;
		}
		#endregion

		#region String
		public static string ConvertToString(object obj)
		{
			if (obj is string)
				return (string)obj;

			return null;
		}
		#endregion

		#region Boolean
		public static bool ConvertToBool(object obj)
		{
			if (obj is bool)
				return (bool)obj;

			return false;
		}
		#endregion
	}
}

