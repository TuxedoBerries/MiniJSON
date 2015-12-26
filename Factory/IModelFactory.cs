using System;

namespace TuxedoBerries.MiniJSON.Factories
{
	public interface IModelFactory
	{
		/// <summary>
		/// Creates a model based on an Object.
		/// </summary>
		/// <returns>The model.</returns>
		/// <param name="data">Data.</param>
		M CreateModel <M>(object data) where M : class, new();
	}
}

