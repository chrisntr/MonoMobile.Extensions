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

		public Tuple<string, Type> GetColumn (MemberInfo member)
		{
			if (member.DeclaringType == typeof(Contact))
				return GetContactColumn (member);
			if (member.DeclaringType == typeof(Email))
				return GetEmailColumn (member);
			if (member.DeclaringType == typeof(Phone))
				return GetPhoneColumn (member);

			return null;
		}

		private Uri table;
		private StringBuilder queryBuilder;
		private IList<string> arguments;

		private Tuple<string, Type> GetPhoneColumn (MemberInfo member)
		{
			return null;
		}

		private Tuple<string, Type> GetEmailColumn (MemberInfo member)
		{
			return null;
		}

		private Tuple<string, Type> GetContactColumn (MemberInfo member)
		{
			switch (member.Name)
			{
				case "DisplayName":
					return new Tuple<string, Type> (ContactsContract.ContactsColumns.DisplayName, typeof(string));
				case "Prefix":
					return new Tuple<string, Type> (ContactsContract.CommonDataKinds.StructuredName.Prefix, typeof(string));
				case "FirstName":
					return new Tuple<string, Type> (ContactsContract.CommonDataKinds.StructuredName.GivenName, typeof(string));
				case "LastName":
					return new Tuple<string, Type> (ContactsContract.CommonDataKinds.StructuredName.FamilyName, typeof(string));
				case "Suffix":
					return new Tuple<string, Type> (ContactsContract.CommonDataKinds.StructuredName.Suffix, typeof(string));

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