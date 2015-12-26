using System;
using System.Reflection;

namespace TuxedoBerries.MiniJSON.Factories.Reflection
{
	[AttributeUsage(AttributeTargets.Property)]
	public class JSONArrayAttribute : JSONBaseAttribute
	{
		public JSONArrayAttribute (string name)
		{
			_specs = new JSONFieldSpecs (name, JSONMemberType.Array);
		}

		public JSONArrayAttribute (string name, string shortName)
		{
			_specs = new JSONFieldSpecs (name, shortName, JSONMemberType.Array);
		}

		public static JSONArrayAttribute GetJSONSpecs(PropertyInfo info)
		{
			var objects = info.GetCustomAttributes (typeof(JSONArrayAttribute), true);
			if (objects == null || objects.Length <= 0)
				return null;

			return objects[0] as JSONArrayAttribute;
		}
	}
}

