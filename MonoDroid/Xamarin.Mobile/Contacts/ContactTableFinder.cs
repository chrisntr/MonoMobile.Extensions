using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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

		public Uri Find (Expression expression, StringBuilder builder, IList<string> argumentList)
		{
			if (builder == null)
				throw new ArgumentNullException ("builder");
			if (argumentList == null)
				throw new ArgumentNullException ("argumentList");

			this.queryBuilder = builder;
			this.arguments = argumentList;

			Visit (expression);

			return this.table;
		}

		public bool IsSupportedType (Type type)
		{
			return type == typeof(Contact) || type == typeof(Phone) || type == typeof (Email);
		}

		private Uri table;
		private StringBuilder queryBuilder;
		private IList<string> arguments;

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
					this.queryBuilder.Append (ContactsContract.DataColumns.Mimetype);
					this.queryBuilder.Append ("=?");

					this.arguments.Add (ContactsContract.CommonDataKinds.StructuredName.ContentItemType);

					return ContactsContract.Data.ContentUri;

				default:
					throw new ArgumentException();
			}
		}
	}
}