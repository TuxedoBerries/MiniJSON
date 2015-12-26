using System;
using System.Reflection;

namespace TuxedoBerries.MiniJSON.Factories.Reflection
{
	public class JSONFieldSpecs
	{
		private PropertyInfo _property;
		private string _name;
		private string _shortName;
		private bool _shortNameAvailable;
		private JSONMemberType _memberType;

		public JSONFieldSpecs (string name) : this(name, JSONMemberType.Value) {}

		public JSONFieldSpecs (string name, JSONMemberType memberType)
		{
			_name = name;
			_shortNameAvailable = false;
			_memberType = memberType;
		}

		public JSONFieldSpecs (string name, string shortName) : this(name, shortName, JSONMemberType.Value) {}

		public JSONFieldSpecs (string name, string shortName, JSONMemberType memberType)
		{
			_name = name;
			_shortName = shortName;
			_shortNameAvailable = true;
			_memberType = memberType;
		}

		/// <summary>
		/// Gets or sets the property mapped to.
		/// </summary>
		/// <value>The name of the property.</value>
		public PropertyInfo Property {
			get {
				return _property;
			}
			set {
				_property = value;
			}
		}

		/// <summary>
		/// Gets the name of the field in a JSON Object.
		/// </summary>
		/// <value>The name.</value>
		public string Name {
			get {
				return _name;
			}
		}

		/// <summary>
		/// Gets the short name of a field in a JSON Object.
		/// </summary>
		/// <value>The short name.</value>
		public string ShortName {
			get {
				return _shortName;
			}
		}

		/// <summary>
		/// Gets a value indicating whether if a Shortname is available or not.
		/// </summary>
		/// <value><c>true</c> if do not use short name; otherwise, <c>false</c>.</value>
		public bool ShortNameAvailable {
			get {
				return _shortNameAvailable;
			}
		}

		/// <summary>
		/// Gets the type of the member.
		/// </summary>
		/// <value>The type of the member.</value>
		public JSONMemberType MemberType {
			get {
				return _memberType;
			}
		}
	}
}

