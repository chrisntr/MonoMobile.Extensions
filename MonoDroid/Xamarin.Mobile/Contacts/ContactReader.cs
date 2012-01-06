using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Content.Res;
using Android.Database;
using Android.Provider;

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
			Android.Net.Uri table = (this.translator != null) 
										? this.translator.Table
										: ((this.rawContacts)
											? ContactsContract.RawContacts.ContentUri
											: ContactsContract.Contacts.ContentUri);

			string[] projections = null;
			if (this.translator != null && this.translator.Projections != null)
			{
				projections = this.translator.Projections.Select (t => t.Item1).ToArray();
				if (projections.Length == 0)
					projections = null;
			}

			string query = null;
			if (this.translator != null)
				query = this.translator.QueryString;

			string[] parameters = null;
			if (this.translator != null)
				parameters = this.translator.ClauseParameters;

			string sortString = null;
			if (this.translator != null)
				sortString = this.translator.SortString;

			ICursor cursor = null;
			try
			{
				cursor = this.content.Query (table, projections, query, parameters, sortString);

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