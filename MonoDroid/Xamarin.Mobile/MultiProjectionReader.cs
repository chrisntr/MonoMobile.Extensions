using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Database;

namespace Xamarin
{
	internal class MultiProjectionReader
		: IEnumerable<object[]>
	{
		internal MultiProjectionReader (ContentResolver content, ContentQueryTranslator translator)
		{
			this.content = content;
			this.translator = translator;
		}

		public IEnumerator<object[]> GetEnumerator()
		{
			Tuple<string, Type>[] projections = translator.Projections.ToArray();

			ICursor cursor = null;
			try
			{
				cursor = this.content.Query (translator.Table, projections.Select (s => s.Item1).ToArray(), translator.QueryString,
				                             translator.ClauseParameters, translator.SortString);

				while (cursor.MoveToNext())
				{
					object[] values = new object[projections.Length];
					for (int i = 0; i < projections.Length; ++i)
					{
						int index = cursor.GetColumnIndex (projections[i].Item1);

						Type t = projections[i].Item2;
						if (t == typeof(string))
							values[i] = cursor.GetString (index);
						else if (t == typeof(int))
							values[i] = cursor.GetInt (index);
						
						yield return values;
					}
				}
			}
			finally
			{
				if (cursor != null)
					cursor.Close();
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private readonly ContentResolver content;
		private readonly ContentQueryTranslator translator;
	}
}