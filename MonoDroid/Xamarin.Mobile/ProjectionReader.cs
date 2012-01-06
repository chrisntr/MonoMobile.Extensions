using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Database;

namespace Xamarin
{
	internal class ProjectionReader<T>
		: IEnumerable<T>
	{
		internal ProjectionReader (ContentResolver content, ContentQueryTranslator translator, Func<ICursor, int, T> selector)
		{
			this.content = content;
			this.translator = translator;
			this.selector = selector;
		}

		public IEnumerator<T> GetEnumerator()
		{
			string[] projections = null;
			if (this.translator.Projections != null)
			{
				projections = this.translator.Projections.Select (t => t.Item1).ToArray();
				if (projections.Length == 0)
					projections = null;
			}

			ICursor cursor = null;
			try
			{
				
				cursor = content.Query (translator.Table, projections,
				                        translator.QueryString, translator.ClauseParameters, translator.SortString);

				while (cursor.MoveToNext())
				{
					int colIndex = cursor.GetColumnIndex (projections[0]);
					yield return this.selector (cursor, colIndex);
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
		private readonly Func<ICursor, int, T> selector;
	}
}