using System;
using System.Collections;
using Android.Content;
using Android.Content.Res;

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

		protected override IEnumerable GetObjectReader (ContentQueryTranslator translator)
		{
			if (translator == null || translator.ReturnType == null || translator.ReturnType == typeof(Contact))
				return new ContactReader (UseRawContacts, translator, content, resources);
			else if (translator.ReturnType == typeof(Phone))
				return new GenericQueryReader<Phone> (translator, content, resources, ContactHelper.GetPhone);
			else if (translator.ReturnType == typeof(Email))
				return new GenericQueryReader<Email> (translator, content, resources, ContactHelper.GetEmail);
			else if (translator.ReturnType == typeof(string))
				return new ProjectionReader<string> (content, translator, (cur,col) => cur.GetString (col));
			else if (translator.ReturnType == typeof(int))
				return new ProjectionReader<int> (content, translator, (cur, col) => cur.GetInt (col));

			throw new ArgumentException();
		}
	}
}