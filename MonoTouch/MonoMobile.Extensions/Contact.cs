using System;
using MonoTouch.AddressBook;
using MonoTouch.Foundation;
 
namespace MonoMobile.Extensions
{
    public class ContactName
    {
        public string Formatted { get; set; }
        public string FamilyName { get; set; }
        public string GivenName { get; set; }
        public string MiddleName { get; set; }
        public string HonorificPrefix { get; set; }
        public string HonorificSuffix { get; set; }
    }
     
    public class ContactField
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public string Pref { get; set; }
        public int Id { get; set; }
    }
     
    public class ContactAddress
    {
        public string Formatted { get; set; }
        public string StreetAddress { get; set; }
        public string Locality { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public int Id { get; set; }
    }
     
    public class ContactOrganization
    {
        public string Name { get; set; }
        public string Dept { get; set; }
        public string Title { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Location { get; set; }
        public string Desc { get; set; }
    }
     
    public class Contact
    {   
		// Please note the individual cross-platform quirks - http://docs.phonegap.com/phonegap_contacts_contacts.md.html#Contact
		
        public bool Success { get; set; }
        public bool Update { get; set; }
        public ABRecord Record { get; set; }
         
         
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public ContactName Name { get; set; }
        public string Nickname { get; set; }
        public ContactField[] PhoneNumbers { get; set; }
        public ContactField[] Emails { get; set; }
        public ContactAddress[] Addresses { get; set; }
        public ContactField[] Ims { get; set; }
        public ContactOrganization[] Organizations { get; set; }
        public string Revision { get; set; }
        public DateTime Birthday { get; set; }
        public string Gender { get; set; }
        public string Note { get; set; }
        public ContactField[] Photos { get; set; }
        public ContactField[] Categories { get; set; }
        public ContactField[] Urls { get; set; }
        public string Timezone { get; set; }
         
        public Contact ()
        {
            Record = new ABPerson();
        }
         
        public Contact (ABRecord record)
        {
            Record = record;
        }
         
        public void Save(Action<Contact> successCallback, Action<string> errorCallback)
        {
			// Should return back the Contact that was saved in the
			// success callback
            throw new NotImplementedException();
        }
		
        public Contact Clone()
        {
            throw new NotImplementedException();
        }
		
		public void Remove()
		{
			throw new NotImplementedException();
		}
		
		// Use this sort of method (as well as a bunch of others) 
		// to bind a contact to a ABPerson record to store in the Address book
		private bool SetContactDataToABRecord()
        {
            var person = Record as ABPerson;
            var success = true;
             
			// Set the rest of the values
			
			// Set Name Info
            // Bind against ContactName
            // Could this be done a more automatic way?
            var nameSet = false;
             
            if(Name != null)
            {
                nameSet = true;
                person.LastName = Name.FamilyName;
                person.FirstName = Name.GivenName;
                person.MiddleName = Name.MiddleName;
                person.Prefix = Name.HonorificPrefix;
                person.Suffix = Name.HonorificSuffix;
            }
             
            if(!String.IsNullOrEmpty(Nickname))
            {
                nameSet = true;
                person.Nickname = Nickname;
            }
             
            if(!nameSet)
            {
                person.Nickname = DisplayName;  
            }
             
			// etc etc for each type of property
			return true;
             
			throw new NotImplementedException();
			
        }
    }
}