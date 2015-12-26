using System;
using System.Reflection;

namespace TuxedoBerries.MiniJSON.Factories.Reflection
{
	[AttributeUsage(AttributeTargets.Property)]
	public class JSONListAttribute : JSONBaseAttribute
	{
		public JSONListAttribute (string name)
		{
			_specs = new JSONFieldSpecs (name, JSONMemberType.List);
		}

		public JSONListAttribute (string name, string shortName)
		{
			_specs = new JSONFieldSpecs (name, shortName, JSONMemberType.List);
		}

		public static JSONListAttribute GetJSONSpecs(PropertyInfo info)
		{
			var objects = info.GetCustomAttributes (typeof(JSONListAttribute), true);
			if (objects == null || objects.Length <= 0)
				return null;

			return objects[0] as JSONListAttribute;
		}
	}
}

