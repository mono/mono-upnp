SUBDIRS = \
	src \
	docs

MAINTAINERCLEANFILES = \
	Makefile.in \
	aclocal.m4 \
	configure \
	config.* \
	INSTALL \
	install-sh \
	missing

EXTRA_DIST = \
	Mono.Upnp.sln

PKV=$(PACKAGE)-$(VERSION)
PKB=$(PKV)-binary

dist-windows: all
	@rm -rf $(PKB) && \
	mkdir -p $(PKB)/bin && \
	cp src/Mono.Ssdp/Mono.Ssdp/Mono.Ssdp.dll src/Mono.Upnp/Mono.Upnp.Client/Mono.Upnp.Client.dll src/Mono.Upnp/Mono.Upnp.Server/Mono.Upnp.Server.dll $(PKB)/bin && \
	cp AUTHORS NEWS README README.Windows $(PKB) && \
	cp COPYING $(PKB)/LICENSE && \
	zip -r9 $(PKB).zip $(PKB) && \
	rm -rf $(PKB)

release:
	@test -e $(PKV).tar.bz2 -a -e $(PKV).tar.gz -a -e $(PKV).zip || { echo "You must run distcheck and dist-windows before release"; exit 1; }; \
	cp ChangeLog $(PKV).changes && \
	sha1sum $(PKV).* $(PKB).* > $(PKV).sha1sum && \
	touch LATEST-IS-$(VERSION) && \
	ssh banshee@banshee-project.org rm /home/banshee/public_html/files/mono-zeroconf/LATEST-IS*; \
	scp $(PKV).* $(PKB).* mono-zeroconf.spec LATEST* README README.Windows banshee@banshee-project.org:public_html/files/mono-zeroconf/ && rm LATEST* $(PKV).* $(PKB).* && \
	test -z $$MONO_USER && { echo "Please set MONO_USER for the commit"; exit 1; }; \
	echo "Sleeping 10 seconds, about to tag release (as user $$MONO_USER)..."; \
	sleep 10; \
	echo "Starting svn tag..."; \
	svn cp svn+ssh://$$MONO_USER@mono-cvs.ximian.com/source/trunk/Mono.Zeroconf svn+ssh://$$MONO_USER@mono-cvs.ximian.com/source/tags/Mono.Zeroconf-$(VERSION) -m 'Created tag for Mono.Zeroconf $(VERSION)';

