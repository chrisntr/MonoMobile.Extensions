MonoMobile.Extensions
=====================

A project to create an abstraction on common mobile parts, such as Contacts and Location.

Heavily influenced by the PhoneGap project.

Implementing the project;

Using Contacts as an example.
http://docs.phonegap.com/phonegap_contacts_contacts.md.html

Should create a IContacts interface to create the Create/Find methods, objects and properties.
The actual implementation should be split into separate class files so;
iOS = Contacts-MonoTouch.cs with the namespace MonoMobile.Extensions.Contacts. 
Android = Contacts-MonoDroid.cs with the namespace MonoMobile.Extensions.Contacts (the both have the same namespace).

For each implementation, it should use its own Class library (such as MonoTouch Class Library etc) and then link to the class files
that makes sense for it.

Hope this helps,

ChrisNTR.