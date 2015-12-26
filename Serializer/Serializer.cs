/*
 * Copyright (c) 2015 Juan Silva
 */

/*
* Copyright (c) 2013 Calvin Rien
*
* Based on the JSON parser by Patrick van Bergen
* http://techblog.procurios.nl/k/618/news/view/14605/14863/How-do-I-write-my-own-parser-for-JSON.html
*
* Simplified it so that it doesn't throw exceptions
* and can be used in Unity iPhone with maximum code stripping.
*
* Permission is hereby granted, free of charge, to any person obtaining
* a copy of this software and associated documentation files (the
	* "Software"), to deal in the Software without restriction, including
* without limitation the rights to use, copy, modify, merge, publish,
* distribute, sublicense, and/or sell copies of the Software, and to
* permit persons to whom the Software is furnished to do so, subject to
	* the following conditions:
	*
	* The above copyright notice and this permission notice shall be
	* included in all copies or substantial portions of the Software.
	*
	* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
	* EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
	* MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
	* IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
	* CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
	* TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
	* SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
	*/
using System;
using System.Text;
using System.Collections;

namespace TuxedoBerries.MiniJSON.Serialization
{
	public sealed class Serializer : IDisposable
	{

		/// <summary>
		/// The builder.
		/// </summary>
		private StringBuilder builder;

		/// <summary>
		/// The object reference.
		/// </summary>
		private object objectRef;

		/// <summary>
		/// Initializes a new instance of the <see cref="MiniJSON.Serializer"/> class.
		/// </summary>
		public Serializer()
		{
			builder = new StringBuilder();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MiniJSON.Serializer"/> class.
		/// </summary>
		/// <param name="obj">Object.</param>
		public Serializer(object obj)
		{
			objectRef = obj;
			builder = new StringBuilder();
		}

		/// <summary>
		/// Serialize the specified obj.
		/// </summary>
		/// <param name="obj">Object.</param>
		public string SerializeThis(object obj)
		{
			SerializeValue (obj);
			return builder.ToString ();
		}

		/// <summary>
		/// Serialize this instance content if given in the constructor.
		/// </summary>
		public string SerializeThis()
		{
			if (objectRef == null)
				return null;

			return SerializeThis (objectRef);
		}

		/// <summary>
		/// Releases all resource used by the <see cref="MiniJSON.Serializer"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="MiniJSON.Serializer"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="MiniJSON.Serializer"/> in an unusable state. After calling
		/// <see cref="Dispose"/>, you must release all references to the <see cref="MiniJSON.Serializer"/> so the garbage
		/// collector can reclaim the memory that the <see cref="MiniJSON.Serializer"/> was occupying.</remarks>
		public void Dispose()
		{
			objectRef = null;
			builder = null;
		}

		#region Serialize By Type
		private void SerializeValue(object value)
		{
			IList asList;
			IDictionary asDict;
			string asStr;

			if (value == null) {
				builder.Append("null");
				return;
			}
			if ((asStr = value as string) != null) {
				SerializeString(asStr);
				return;
			}
			if (value is bool) {
				builder.Append((bool) value ? "true" : "false");
				return;
			}
			if ((asList = value as IList) != null) {
				SerializeArray(asList);
				return;
			}
			if ((asDict = value as IDictionary) != null) {
				SerializeObject(asDict);
				return;
			}
			if (value is char) {
				SerializeString(new string((char) value, 1));
				return;
			}

			SerializeOther(value);
		}

		private void SerializeObject(IDictionary obj)
		{
			bool first = true;

			builder.Append('{');

			foreach (object e in obj.Keys) {
				if (!first) {
					builder.Append(',');
				}

				SerializeString(e.ToString());
				builder.Append(':');

				SerializeValue(obj[e]);

				first = false;
			}

			builder.Append('}');
		}

		private void SerializeArray(IList anArray)
		{
			builder.Append('[');

			bool first = true;

			foreach (object obj in anArray) {
				if (!first) {
					builder.Append(',');
				}

				SerializeValue(obj);

				first = false;
			}

			builder.Append(']');
		}

		private void SerializeString(string str)
		{
			builder.Append('\"');

			char[] charArray = str.ToCharArray();
			foreach (var c in charArray) {
				switch (c) {
				case '"':
					builder.Append("\\\"");
					break;
				case '\\':
					builder.Append("\\\\");
					break;
				case '\b':
					builder.Append("\\b");
					break;
				case '\f':
					builder.Append("\\f");
					break;
				case '\n':
					builder.Append("\\n");
					break;
				case '\r':
					builder.Append("\\r");
					break;
				case '\t':
					builder.Append("\\t");
					break;
				default:
					int codepoint = Convert.ToInt32(c);
					if ((codepoint >= 32) && (codepoint <= 126)) {
						builder.Append(c);
					} else {
						builder.Append("\\u");
						builder.Append(codepoint.ToString("x4"));
					}
					break;
				}
			}

			builder.Append('\"');
		}

		private void SerializeOther(object value)
		{
			// NOTE: decimals lose precision during serialization.
			// They always have, I'm just letting you know.
			// Previously floats and doubles lost precision too.
			if (value is float) {
				builder.Append(((float) value).ToString("R"));
				return;
			}
			if (value is int
				|| value is uint
				|| value is long
				|| value is sbyte
				|| value is byte
				|| value is short
				|| value is ushort
				|| value is ulong) {
				builder.Append(value);
				return;
			}
			if (value is double
				|| value is decimal) {
				builder.Append(Convert.ToDouble(value).ToString("R"));
				return;
			}

			SerializeString(value.ToString());
		}
		#endregion

		#region Static Functions
		/// <summary>
		/// Serialize the specified obj.
		/// </summary>
		/// <param name="obj">Object.</param>
		public static string Serialize(object obj)
		{
			var instance = new Serializer();

			return instance.SerializeThis(obj);
		}
		#endregion
	}
}

