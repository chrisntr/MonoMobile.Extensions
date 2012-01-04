using System.Collections.Generic;
using Android.Content;
using Android.Content.Res;
using Android.Database;

namespace Xamarin.Contacts
{
	internal class ContactQueryProvider
		: ContentQueryProvider<Contact>
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

		protected override Contact GetElement (ICursor cursor)
		{
			return ContactHelper.GetContact (UseRawContacts, this.content, this.resources, cursor);
		}

		protected override IEnumerable<Contact> GetElements ()
		{
			return ContactHelper.GetContacts (UseRawContacts, this.content, this.resources);
		}
	}
}