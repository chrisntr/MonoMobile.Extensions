using System;
using Xamarin.Contacts;

public class CodeSamples
{
	public CodeSamples()
	{
		#region AddressBook
		var builder = new StringBuilder();
		var abook = new AddressBook();
		foreach (Contact c in abook.Where (c => c.FirstName == "Eric" && c.Phones.Any()))
		{
			builder.AppendLine (c.DisplayName);
			foreach (Phone p in c.Phones)
				builder.AppendLine (String.Format ("{0}: {1}", p.Label, p.Number));

			builder.AppendLine();
		}
		#endregion
	}
}