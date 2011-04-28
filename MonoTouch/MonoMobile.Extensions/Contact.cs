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
         
        public void Save()
        {
             
//          NSNumber* cId = [contactDict valueForKey:kW3ContactId];
//          Contact* aContact = nil; 
//          ABRecordRef rec = nil;
//          if (cId && ![cId isKindOfClass:[NSNull class]]){
//              rec = ABAddressBookGetPersonWithRecordID(addrBook, [cId intValue]);
//              if (rec){
//                  aContact = [[Contact alloc] initFromABRecord: rec ];
//                  bUpdate = YES;
//              }
//          }
//          if (!aContact){
//              aContact = [[Contact alloc] init];          
//          }
            Contact aContact = null;
            ABAddressBook addrBook = new ABAddressBook();
            ABRecord rec = null;
             
            if(Id != null)
            {
                rec = addrBook.GetPerson(Id);
                if(rec != null)
                {
                    Record = rec;
                    //this = new Contact(rec);
                    Update = true;
                }
            }

			// Convert to a ABPerson (ABRecord) - might need update or just a new one creating...
            this.SetContactDataToABRecord();
            //Success = aContact.
           
            //addrBook.Save();
             
             
        }
         
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
             
			return true;
             
            // Set Address Book...
             
//            ABMutableDictionaryMultiValue addresses = new ABMutableDictionaryMultiValue();
//            NSMutableDictionary a = new NSMutableDictionary();
//     
//            a.Add(new NSString(ABPersonAddressKey.City), new NSString(city));
//            a.Add(new NSString(ABPersonAddressKey.State), new NSString(state));
//            a.Add(new NSString(ABPersonAddressKey.Zip), new NSString(zip));
//            a.Add(new NSString(ABPersonAddressKey.Street), new NSString(addr1));
//     
//            addresses.Add(a, new NSString("Home"));
//            p.SetAddresses(addresses);
//            // Set PhoneNumbers
//            foreach(var address in Addresses)
//            {
//                 
//                var multiValueAddress = ABMultiValue<Address 
//            }
//            var addres = person.GetAddresses();
//            addres[
////          
//          /NSLog(@"setting phoneNumbers");
//  NSArray* array = [aContact valueForKey:kW3ContactPhoneNumbers];
//  if ([array isKindOfClass:[NSArray class]]){
//      [self setMultiValueStrings: array forProperty: kABPersonPhoneProperty inRecord: person asUpdate: bUpdate];
//  }
             
             
//          id nn = [aContact valueForKey:kW3ContactNickname];
//  if (![nn isKindOfClass:[NSNull class]]){
//      bName = true;
//      [self setValue: nn forProperty: kABPersonNicknameProperty inRecord: person asUpdate: bUpdate];
//  }
//  if (!bName){
//      // if no name or nickname - try and use displayName as W3Contact must have displayName or ContactName
//      [self setValue:[aContact valueForKey:kW3ContactDisplayName] forProperty: kABPersonNicknameProperty 
//                       inRecord: person asUpdate: bUpdate];
//  }
//          
//          ABRecordRef person = self.record;
//  bool bSuccess= TRUE;
//  CFErrorRef error;
//
//  // set name info
//  // iOS doesn't have displayName - might have to pull parts from it to create name
//  bool bName = false;
//  NSMutableDictionary* dict = [aContact valueForKey:kW3ContactName];
//  if ([dict isKindOfClass:[NSDictionary class]]){
//      bName = true;
//      NSArray* propArray = [[Contact defaultObjectAndProperties] objectForKey: kW3ContactName];
//      for(id i in propArray){
//          if (![(NSString*)i isEqualToString:kW3ContactFormattedName]){  //kW3ContactFormattedName is generated from ABRecordCopyCompositeName() and can't be set
//              [self setValue:[dict valueForKey:i] forProperty: (ABPropertyID)[(NSNumber*)[[Contact defaultW3CtoAB] objectForKey: i]intValue] 
//                  inRecord: person asUpdate: bUpdate];
//          }
//      }
//  }
             
//          bool bSuccess = true;  // if property was null, just ignore and return success
//  CFErrorRef error;
//  if (aValue && ![aValue isKindOfClass:[NSNull class]]){
//      if (bUpdate && ([aValue isKindOfClass:[NSString class]] &&  [aValue length] == 0)) { // if updating, empty string means to delete
//          aValue = NULL;
//      } // really only need to set if different - more efficient to just update value or compare and only set if necessay???
//      bSuccess = ABRecordSetValue(aRecord, aProperty, aValue, &error);
//      if (!bSuccess){
//          NSLog(@"error setting @% property", aProperty);
//      }
//  }
//
//  return bSuccess;
        }
         
        public void Clone()
        {
             
        }
    }
}