using System;
using System.Collections;
using System.Collections.Generic;
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
			ICursor cursor = null;
			try
			{
				cursor = content.Query (translator.Table, translator.Projections, translator.QueryString,
				                        translator.ClauseParameters, translator.SortString);

				while (cursor.MoveToNext())
				{
					int colIndex = cursor.GetColumnIndex (this.translator.Projections[0]);
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