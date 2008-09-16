DOC_UPDATER = @MONODOCER@ -delete
DOC_ASSEMBLER = @MDASSEMBLER@ --out $(DOC_PACKAGE) --ecma

ASSEMBLIES_BUILD = $(foreach asm,$(DOC_ASSEMBLIES),$(addprefix -assembly:,$(asm)))

if BUILD_DOCS

monodocdir = $(DOCDIR)
monodoc_DATA = \
	$(DOC_PACKAGE).zip \
	$(DOC_PACKAGE).tree \
	$(DOC_PACKAGE).source

$(DOC_PACKAGE).zip $(DOC_PACKAGE).tree: $(srcdir)/en/*/*.xml $(srcdir)/en/*.xml
	$(DOC_ASSEMBLER) $(srcdir)/en

update-docs: $(ASSEMBLIES)
	$(DOC_UPDATER) $(ASSEMBLIES_BUILD) -path:en/

update-svn:
	@for remove in $$(find en -iregex .*\.remove$$); do \
		real_remove=$${remove%.remove}; \
		mv $$remove $$real_remove; \
		svn delete $$real_remove; \
	done; \
	for add in $$(svn status | grep ^? | awk '{print $$2}'); do \
		svn add $$add; \
	done;

endif

merge:
	monodoc --merge-changes $$HOME/.config/monodoc/changeset.xml .

EXTRA_DIST = \
	$(srcdir)/en/*/*.xml \
	$(srcdir)/en/*.xml \
	$(DOC_PACKAGE).source
	
DISTCLEANFILES = \
	$(DOC_PACKAGE).zip \
	$(DOC_PACKAGE).tree

MAINTAINERCLEANFILES = \
	Makefile.in

