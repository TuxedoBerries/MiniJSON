using System;
using System.Reflection;

namespace TuxedoBerries.MiniJSON.Factories.Reflection
{
	[AttributeUsage(AttributeTargets.Property)]
	public class JSONObjectAttribute : JSONBaseAttribute
	{
		public JSONObjectAttribute (string name)
		{
			_specs = new JSONFieldSpecs (name, JSONMemberType.Object);
		}

		public JSONObjectAttribute (string name, string shortName)
		{
			_specs = new JSONFieldSpecs (name, shortName, JSONMemberType.Object);
		}

		public static JSONObjectAttribute GetJSONSpecs(PropertyInfo info)
		{
			var objects = info.GetCustomAttributes (typeof(JSONObjectAttribute), true);
			if (objects == null || objects.Length <= 0)
				return null;

			return objects[0] as JSONObjectAttribute;
		}
	}
}

