using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Android.Provider;
using Uri = Android.Net.Uri;

namespace Xamarin.Contacts
{
	internal class ContactTableFinder
		: ExpressionVisitor, ITableFinder
	{
		public bool UseRawContacts
		{
			get;
			set;
		}

		public Uri DefaultTable
		{
			get { return (UseRawContacts) ? ContactsContract.RawContacts.ContentUri : ContactsContract.Contacts.ContentUri; }
		}

		public TableFindResult Find (Expression expression)
		{
			this.queryBuilder.Clear();
			this.arguments.Clear();

			Visit (expression);

			return new TableFindResult (this.table, this.queryBuilder.ToString(), this.arguments.ToArray());
		}

		public bool IsSupportedType (Type type)
		{
			return type == typeof(Contact)
				|| type == typeof(Phone)
				|| type == typeof(Email)
				|| type == typeof(Address)
				|| type == typeof(Relationship);
		}

		public ContentResolverColumnMapping GetColumn (MemberInfo member)
		{
			if (member.DeclaringType == typeof(Contact))
				return GetContactColumn (member);
			if (member.DeclaringType == typeof(Email))
				return GetEmailColumn (member);
			if (member.DeclaringType == typeof(Phone))
				return GetPhoneColumn (member);
			if (member.DeclaringType == typeof(Address))
				return GetAddressColumn (member);
			if (member.DeclaringType == typeof(Relationship))
				return GetRelationshipColumn (member);
			if (member.DeclaringType == typeof(InstantMessagingAccount))
				return GetImColumn (member);

			return null;
		}

		private ContentResolverColumnMapping GetImColumn (MemberInfo member)
		{
			return null;
		}

		private Uri table;
		private readonly StringBuilder queryBuilder = new StringBuilder();
		private readonly List<string> arguments = new List<string>();

		private ContentResolverColumnMapping GetRelationshipColumn (MemberInfo member)
		{
			switch (member.Name)
			{
				case "Name":
					return new ContentResolverColumnMapping (ContactsContract.CommonDataKinds.Relation.Name, typeof (string));
			}

			return null;
		}

		private ContentResolverColumnMapping GetAddressColumn (MemberInfo member)
		{
			switch (member.Name)
			{
				case "City":
					return new ContentResolverColumnMapping (ContactsContract.CommonDataKinds.StructuredPostal.City, typeof(string));
				case "Region":
					return new ContentResolverColumnMapping (ContactsContract.CommonDataKinds.StructuredPostal.Region, typeof (string));
				case "Country":
					return new ContentResolverColumnMapping (ContactsContract.CommonDataKinds.StructuredPostal.Country, typeof (string));
				case "PostalCode":
					return new ContentResolverColumnMapping (ContactsContract.CommonDataKinds.StructuredPostal.Postcode, typeof(string));
			}

			return null;
		}

		private ContentResolverColumnMapping GetPhoneColumn (MemberInfo member)
		{
			switch (member.Name)
			{
				case "Number":
					return new ContentResolverColumnMapping (ContactsContract.CommonDataKinds.Phone.Number, typeof (string));
			}

			return null;
		}

		private ContentResolverColumnMapping GetEmailColumn (MemberInfo member)
		{
			switch (member.Name)
			{
				case "Address":
					return new ContentResolverColumnMapping (ContactsContract.DataColumns.Data1, typeof (string));
			}

			return null;
		}

		private ContentResolverColumnMapping GetContactColumn (MemberInfo member)
		{
			switch (member.Name)
			{
				case "DisplayName":
					return new ContentResolverColumnMapping (ContactsContract.ContactsColumns.DisplayName, typeof(string));
				case "Prefix":
					return new ContentResolverColumnMapping (ContactsContract.CommonDataKinds.StructuredName.Prefix, typeof(string));
				case "FirstName":
					return new ContentResolverColumnMapping (ContactsContract.CommonDataKinds.StructuredName.GivenName, typeof(string));
				case "LastName":
					return new ContentResolverColumnMapping (ContactsContract.CommonDataKinds.StructuredName.FamilyName, typeof(string));
				case "Suffix":
					return new ContentResolverColumnMapping (ContactsContract.CommonDataKinds.StructuredName.Suffix, typeof(string));
				case "Phones":
					return new ContentResolverColumnMapping ((string)null, typeof (IEnumerable<Phone>));
				case "Emails":
					return new ContentResolverColumnMapping ((string)null, typeof (IEnumerable<Email>));
				case "Addresses":
					return new ContentResolverColumnMapping ((string)null, typeof (IEnumerable<Address>));

				default:
					return null;
			}
		}

		protected override Expression VisitMemberAccess (MemberExpression member)
		{
			member = (MemberExpression)base.VisitMemberAccess (member);

			if (this.table == null)
			{
				if (member.Member.DeclaringType == typeof (Contact))
					this.table = GetContactTable (member);
				else if (member.Member.DeclaringType == typeof(Phone))
					this.table = ContactsContract.CommonDataKinds.Phone.ContentUri;
				else if (member.Member.DeclaringType == typeof(Email))
					this.table = ContactsContract.CommonDataKinds.Email.ContentUri;
			}

			return member;
		}

		private void AddMimeConstraint (string type)
		{
			this.queryBuilder.Append (ContactsContract.DataColumns.Mimetype);
			this.queryBuilder.Append ("=?");

			this.arguments.Add (type);
		}

		private Uri GetContactTable (MemberExpression expression)
		{
			switch (expression.Member.Name)
			{
				case "DisplayName":
					return (UseRawContacts) ? ContactsContract.RawContacts.ContentUri : ContactsContract.Contacts.ContentUri;
				
				case "Prefix":
				case "FirstName":
				case "MiddleName":
				case "LastName":
				case "Suffix":
					AddMimeConstraint (ContactsContract.CommonDataKinds.StructuredName.ContentItemType);
					return ContactsContract.Data.ContentUri;

				case "Relationships":
					AddMimeConstraint (ContactsContract.CommonDataKinds.Relation.ContentItemType);
					return ContactsContract.Data.ContentUri;

				case "Organizations":
					AddMimeConstraint (ContactsContract.CommonDataKinds.Organization.ContentItemType);
					return ContactsContract.Data.ContentUri;

				case "Notes":
					AddMimeConstraint (ContactsContract.CommonDataKinds.Note.ContentItemType);
					return ContactsContract.Data.ContentUri;

				case "Phones":
					return ContactsContract.CommonDataKinds.Phone.ContentUri;
				case "Emails":
					return ContactsContract.CommonDataKinds.Email.ContentUri;
				case "Addresses":
					return ContactsContract.CommonDataKinds.StructuredPostal.ContentUri;

				case "Websites":
					AddMimeConstraint (ContactsContract.CommonDataKinds.Website.ContentItemType);
					return ContactsContract.Data.ContentUri;

				case "InstantMessagingAccounts":
					AddMimeConstraint (ContactsContract.CommonDataKinds.Im.ContentItemType);
					return ContactsContract.Data.ContentUri;

				default:
					return null;
			}
		}
	}
}