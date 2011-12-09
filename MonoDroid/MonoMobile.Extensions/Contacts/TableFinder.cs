using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Android.Provider;
using Uri = Android.Net.Uri;

namespace Xamarin.Contacts
{
	internal class TableFinder
		: ExpressionVisitor
	{
		private TableFinder (bool rawContacts, StringBuilder queryBuilder, List<string> arguments)
		{
			this.rawContacts = rawContacts;
			this.queryBuilder = queryBuilder;
			this.arguments = arguments;
		}

		private Uri table;
		private readonly bool rawContacts;
		private readonly StringBuilder queryBuilder;
		private readonly List<string> arguments;

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
					return (this.rawContacts) ? ContactsContract.RawContacts.ContentUri : ContactsContract.Contacts.ContentUri;
				
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

		public static Uri Find (Expression expression, bool rawContacts, StringBuilder queryBuilder, List<string> arguments)
		{
			var finder = new TableFinder (rawContacts, queryBuilder, arguments);
			finder.Visit (expression);
			return finder.table;
		}
	}
}