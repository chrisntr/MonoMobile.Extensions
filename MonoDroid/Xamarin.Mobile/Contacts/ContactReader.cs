using System;
using System.Collections;
using System.Collections.Generic;
using Android.Content;
using Android.Content.Res;
using Android.Database;

namespace Xamarin.Contacts
{
	internal class ContactReader
		: IEnumerable<Contact>
	{
		public ContactReader (bool useRawContacts, ContentQueryTranslator translator, ContentResolver content, Resources resources)
		{
			this.rawContacts = useRawContacts;
			this.translator = translator;
			this.content = content;
			this.resources = resources;
		}

		public IEnumerator<Contact> GetEnumerator()
		{
			ICursor cursor = null;
			try
			{
				cursor = this.content.Query (this.translator.Table, this.translator.Projections, this.translator.QueryString,
				                             this.translator.ClauseParameters, this.translator.SortString);

				while (cursor.MoveToNext())
					yield return ContactHelper.GetContact (this.rawContacts, this.content, this.resources, cursor);
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

		private readonly bool rawContacts;
		private readonly ContentQueryTranslator translator;
		private readonly ContentResolver content;
		private readonly Resources resources;
	}
}