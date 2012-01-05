using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Content.Res;
using Android.Database;

namespace Xamarin.Contacts
{
	internal class ContactQueryProvider
		: ContentQueryProvider
	{
		internal ContactQueryProvider (ContentResolver content, Resources resources)
			: base (content, resources, new ContactTableFinder())
		{
		}

		public bool UseRawContacts
		{
			get { return ((ContactTableFinder)TableFinder).UseRawContacts; }
			set { ((ContactTableFinder)TableFinder).UseRawContacts = value; }
		}

		protected override IEnumerable GetObjectReader (ContentResolver content, Resources resources, ContentQueryTranslator translator)
		{
			if (translator.ReturnType == null || translator.ReturnType == typeof(Contact))
				return new ContactReader (UseRawContacts, translator, content, resources);
			else if (translator.ReturnType == typeof(string))
				return new ProjectionReader<string> (content, translator, (cur,col) => cur.GetString (col));
			else if (translator.ReturnType == typeof(int))
				return new ProjectionReader<int> (content, translator, (cur, col) => cur.GetInt (col));

			throw new ArgumentException();
		}
	}
}