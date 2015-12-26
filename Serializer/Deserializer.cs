﻿/*
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
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace TuxedoBerries.MiniJSON.Serialization
{
	public sealed class Deserializer : IDisposable
	{

		private const string WORD_BREAK = "{}[],:\"";

		/// <summary>
		/// The json.
		/// </summary>
		private StringReader json;

		/// <summary>
		/// Initializes a new instance of the <see cref="MiniJSON.Deserializer"/> class.
		/// </summary>
		public Deserializer()
		{
			// Empty constructor
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MiniJSON.Deserializer"/> class.
		/// </summary>
		/// <param name="jsonString">Json string.</param>
		public Deserializer(string jsonString)
		{
			json = new StringReader(jsonString);
		}

		/// <summary>
		/// Releases all resource used by the <see cref="MiniJSON.Deserializer"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="MiniJSON.Deserializer"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="MiniJSON.Deserializer"/> in an unusable state. After calling
		/// <see cref="Dispose"/>, you must release all references to the <see cref="MiniJSON.Deserializer"/> so the garbage
		/// collector can reclaim the memory that the <see cref="MiniJSON.Deserializer"/> was occupying.</remarks>
		public void Dispose()
		{
			json.Dispose();
			json = null;
		}

		/// <summary>
		/// Parse the specified jsonString.
		/// </summary>
		/// <param name="jsonString">Json string.</param>
		public object Parse(string jsonString)
		{
			json = new StringReader(jsonString);
			return ParseValue ();
		}

		/// <summary>
		/// Parse this instance.
		/// </summary>
		public object Parse()
		{
			return ParseValue ();
		}

		#region Parse by Type
		private Dictionary<string, object> ParseObject()
		{
			Dictionary<string, object> table = new Dictionary<string, object>();

			// ditch opening brace
			json.Read();

			// {
			while (true) {
				switch (NextToken) {
				case TOKEN.NONE:
					return null;
				case TOKEN.COMMA:
					continue;
				case TOKEN.CURLY_CLOSE:
					return table;
				default:
					// name
					string name = ParseString();
					if (name == null) {
						return null;
					}

					// :
					if (NextToken != TOKEN.COLON) {
						return null;
					}
					// ditch the colon
					json.Read();

					// value
					table[name] = ParseValue();
					break;
				}
			}
		}

		private List<object> ParseArray()
		{
			List<object> array = new List<object>();

			// ditch opening bracket
			json.Read();

			// [
			var parsing = true;
			while (parsing) {
				TOKEN nextToken = NextToken;

				switch (nextToken) {
				case TOKEN.NONE:
					return null;
				case TOKEN.COMMA:
					continue;
				case TOKEN.SQUARED_CLOSE:
					parsing = false;
					break;
				default:
					object value = ParseByToken(nextToken);

					array.Add(value);
					break;
				}
			}

			return array;
		}

		private object ParseValue()
		{
			TOKEN nextToken = NextToken;
			return ParseByToken(nextToken);
		}

		private object ParseByToken(TOKEN token)
		{
			switch (token) {
				case TOKEN.STRING:
					return ParseString();
				case TOKEN.NUMBER:
					return ParseNumber();
				case TOKEN.CURLY_OPEN:
					return ParseObject();
				case TOKEN.SQUARED_OPEN:
					return ParseArray();
				case TOKEN.TRUE:
					return true;
				case TOKEN.FALSE:
					return false;
				case TOKEN.NULL:
					return null;
				default:
					return null;
			}
		}

		private string ParseString()
		{
			StringBuilder s = new StringBuilder();
			char c;

			// ditch opening quote
			json.Read();

			bool parsing = true;
			while (parsing) {

				if (json.Peek() == -1) {
					parsing = false;
					break;
				}

				c = NextChar;
				switch (c) {
				case '"':
					parsing = false;
					break;
				case '\\':
					if (json.Peek() == -1) {
						parsing = false;
						break;
					}

					c = NextChar;
					switch (c) {
					case '"':
					case '\\':
					case '/':
						s.Append(c);
						break;
					case 'b':
						s.Append('\b');
						break;
					case 'f':
						s.Append('\f');
						break;
					case 'n':
						s.Append('\n');
						break;
					case 'r':
						s.Append('\r');
						break;
					case 't':
						s.Append('\t');
						break;
					case 'u':
						var hex = new char[4];

						for (int i=0; i< 4; i++) {
							hex[i] = NextChar;
						}

						s.Append((char) Convert.ToInt32(new string(hex), 16));
						break;
					}
					break;
				default:
					s.Append(c);
					break;
				}
			}

			return s.ToString();
		}

		private object ParseNumber()
		{
			string number = NextWord;

			if (number.IndexOf('.') == -1) {
				long parsedInt;
				Int64.TryParse(number, out parsedInt);
				return parsedInt;
			}

			double parsedDouble;
			Double.TryParse(number, out parsedDouble);
			return parsedDouble;
		}
		#endregion

		#region Helpers
		private bool IsWordBreak(char c)
		{
			return Char.IsWhiteSpace(c) || WORD_BREAK.IndexOf(c) != -1;
		}

		private void EatWhitespace()
		{
			while (Char.IsWhiteSpace(PeekChar)) {
				json.Read();

				if (json.Peek() == -1) {
					break;
				}
			}
		}
		#endregion

		#region Accessors
		private char PeekChar {
			get {
				return Convert.ToChar(json.Peek());
			}
		}

		private char NextChar {
			get {
				return Convert.ToChar(json.Read());
			}
		}

		private string NextWord {
			get {
				StringBuilder word = new StringBuilder();

				while (!IsWordBreak(PeekChar)) {
					word.Append(NextChar);

					if (json.Peek() == -1) {
						break;
					}
				}

				return word.ToString();
			}
		}

		private TOKEN NextToken {
			get {
				EatWhitespace();

				if (json.Peek() == -1) {
					return TOKEN.NONE;
				}

				switch (PeekChar) {
				case '{':
					return TOKEN.CURLY_OPEN;
				case '}':
					json.Read();
					return TOKEN.CURLY_CLOSE;
				case '[':
					return TOKEN.SQUARED_OPEN;
				case ']':
					json.Read();
					return TOKEN.SQUARED_CLOSE;
				case ',':
					json.Read();
					return TOKEN.COMMA;
				case '"':
					return TOKEN.STRING;
				case ':':
					return TOKEN.COLON;
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
				case '-':
					return TOKEN.NUMBER;
				}

				switch (NextWord) {
				case "false":
					return TOKEN.FALSE;
				case "true":
					return TOKEN.TRUE;
				case "null":
					return TOKEN.NULL;
				}

				return TOKEN.NONE;
			}
		}
		#endregion

		#region Static Functions
		/// <summary>
		/// Deserialize the specified jsonString.
		/// </summary>
		/// <param name="jsonString">Json string.</param>
		public static object Deserialize(string jsonString)
		{
			using (var instance = new Deserializer(jsonString)) {
				return instance.ParseValue();
			}
		}
		#endregion
	}
}

