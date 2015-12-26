using System;
using System.Reflection;

namespace TuxedoBerries.MiniJSON.Factories.Reflection
{
	[AttributeUsage(AttributeTargets.Property)]
	public class JSONFieldAttribute : JSONBaseAttribute
	{
		public JSONFieldAttribute (string name)
		{
			_specs = new JSONFieldSpecs (name);
		}

		public JSONFieldAttribute (string name, string shortName)
		{
			_specs = new JSONFieldSpecs (name, shortName);
		}

		public static JSONFieldAttribute GetJSONSpecs(PropertyInfo info)
		{
			var objects = info.GetCustomAttributes (typeof(JSONFieldAttribute), true);
			if (objects == null || objects.Length <= 0)
				return null;

			return objects[0] as JSONFieldAttribute;
		}
	}
}

