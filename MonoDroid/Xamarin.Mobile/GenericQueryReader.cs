using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Content.Res;
using Android.Database;
using Android.Provider;

namespace Xamarin
{
	internal class GenericQueryReader<T>
		: IEnumerable<T>
	{
		public GenericQueryReader (ContentQueryTranslator translator, ContentResolver content, Resources resources, Func<ICursor, Resources, T> selector)
		{
			if (translator == null)
				throw new ArgumentNullException ("translator");
			if (content == null)
				throw new ArgumentNullException ("content");
			if (resources == null)
				throw new ArgumentNullException ("resources");
			if (selector == null)
				throw new ArgumentNullException ("selector");

			this.translator = translator;
			this.content = content;
			this.resources = resources;
			this.selector = selector;
		}

		public IEnumerator<T> GetEnumerator()
		{
			ICursor cursor = null;
			try
			{
				string[] projections = (translator.Projections != null)
				                       	? this.translator.Projections.Select (p => p.Item1).ToArray()
				                       	: null;

				cursor = this.content.Query (this.translator.Table, projections,
				                             this.translator.QueryString, this.translator.ClauseParameters,
				                             this.translator.SortString);

				while (cursor.MoveToNext())
					yield return this.selector (cursor, this.resources);
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

		private readonly Func<ICursor, Resources, T> selector;
		private readonly ContentQueryTranslator translator;
		private readonly ContentResolver content;
		private readonly Resources resources;
	}
}