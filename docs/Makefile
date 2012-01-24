DOCS_ASSEMBLIES=../MonoDroid/Xamarin.Mobile/bin/Debug/Xamarin.Mobile.dll

update:
	mdoc update --delete $(MDOC_UPDATE_OPTIONS) -o en $(DOCS_ASSEMBLIES)
publish:
	mdoc export-msxdoc -o Xamarin.Mobile.xml en
	mdoc assemble --debug -o Xamarin.Mobile en
