using System;

namespace TuxedoBerries.MiniJSON.Factories.Reflection
{
	public abstract class JSONBaseAttribute : Attribute
	{
		protected JSONFieldSpecs _specs;

		/// <summary>
		/// Gets the specs.
		/// </summary>
		/// <value>The specs.</value>
		public JSONFieldSpecs Specs {
			get {
				return _specs;
			}
		}
	}
}

